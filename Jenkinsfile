// Generic Webhook Trigger Plugin
// SSH Agent Plugin
pipeline {
    agent any
    stages {
        // stage('Clone repository') {
        //     steps {
        //          git branch: 'main', credentialsId: 'Ndkn', url: 'https://github.com/ndknitor/JenkinsImpact'
        //     }
        // }
        stage('Build') {
            when {
                expression { params.CD == "Development" || params.CD == "PassProduction" }
            }
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --no-restore'
            }
        }
        stage('Test') {
            when {
                expression { params.CD == "Development" || params.CD == "PassProduction" }
            }
            steps {
                sh 'dotnet test'
            }
        }
        stage('Deploy development') {
            when {
                expression { params.CD == "Development" || params.CD == "PassProduction" }
            }
            steps {
                sshagent(['ssh-remote']) {
                    sh '''
                        ssh vagrant@192.168.56.82 \
                        "cd AspTemplate &&
                        git pull &&
                        docker build -t debian3:5000/asp-template:dev . &&
                        docker stop asp-template &&
                        docker rm asp-template &&
                        docker run --name asp-template -e ASPNETCORE_ENVIRONMENT="Development" --restart=always -d -p 8080:8080 debian3:5000/asp-template:dev &&
                        docker push debian3:5000/asp-template:dev
                        docker image prune -f"
                    '''
                }
            }
            
        }
        stage('Deploy staging') {
            when {
                expression { params.CD == "Staging" || params.CD == "PassProduction" }
            }
            steps {
                sshagent(['ssh-remote']) {
                    sh '''
                        ssh -o StrictHostKeyChecking=no vagrant@192.168.56.83 \
                        "docker pull debian3:5000/asp-template:dev &&
                        docker tag debian3:5000/asp-template:dev debian3:5000/asp-template &&
                        docker stop asp-template &&
                        docker rm asp-template &&
                        docker run --name asp-template -e ASPNETCORE_ENVIRONMENT="Staging" --restart=always -d -p 8080:8080 debian3:5000/asp-template &&
                        docker push debian3:5000/asp-template &&
                        docker image prune -f"
                    '''
                }
            }
        }
        stage('Deploy production') {
            when {
                expression { params.CD == "Production" || params.CD == "PassProduction" }
            }
            steps {
                sshagent(['ssh-remote']) {
                    sh '''
                        ssh -o StrictHostKeyChecking=no vagrant@192.168.56.84 \
                        "export KUBECONFIG=/home/vagrant/.kube/config.yml &&
                        kubectl rollout restart deployment/asp-template-deployment"
                    '''
                }
            }
        }
    }
    post {
        always {
            echo "Clean up"
        }
    }
}