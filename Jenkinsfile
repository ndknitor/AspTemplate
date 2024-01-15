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
                expression { params.CD == "Development"}
            }
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --no-restore'
            }
        }
        stage('Test') {
            when {
                expression { params.CD == "Development"}
            }
            parallel {
                stage('Unit Tests') {
                    steps {
                        echo 'Running unit tests...'
                        // Add your unit test steps here
                    }
                }
                stage('Integration Tests') {
                    steps {
                        echo 'Running integration tests...'
                        // Add your integration test steps here
                    }
                }
            }
            // steps {
            //     sh 'dotnet test'
            // }
        }
        stage('Deploy development') {
            when {
                expression { params.CD == "Development" }
            }
            steps {
                sshagent(['ssh-remote']) {
                    sh '''
                        ssh -o StrictHostKeyChecking=no vagrant@192.168.56.82 \
                        "cd AspTemplate 
                        git pull 
                        docker build -t debian3:5000/asp-template:dev . 
                        docker stop asp-template 
                        docker rm asp-template 
                        docker run --name asp-template -e ASPNETCORE_ENVIRONMENT="Development" --restart=always -d -p 8080:8080 debian3:5000/asp-template:dev 
                        docker push debian3:5000/asp-template:dev
                        docker image prune -f"
                    '''
                }
            }
            
        }
        stage('Deploy staging') {
            when {
                expression { params.CD == "Staging" || params.CD == "PassProduction" || params.Auto}
            }
            steps {
                sshagent(['ssh-remote']) {
                    sh '''
                        ssh -o StrictHostKeyChecking=no vagrant@192.168.56.83 \
                        "docker pull debian3:5000/asp-template:dev 
                        docker tag debian3:5000/asp-template:dev debian3:5000/asp-template 
                        docker stop asp-template 
                        docker rm asp-template 
                        docker run --name asp-template -e ASPNETCORE_ENVIRONMENT="Staging" --restart=always -d -p 10000:8080 debian3:5000/asp-template 
                        docker push debian3:5000/asp-template 
                        docker image prune -f 
                        trivy image debian3:5000/asp-template"
                    '''
                }
            }
        }
        stage('Scan') {
            when {
                expression { params.CD == "Staging" || params.CD == "PassProduction" || params.Auto}
            }
            parallel {
                stage('Image scan') {
                    steps {
                        sshagent(['ssh-remote']) {
                            sh '''
                                ssh -o StrictHostKeyChecking=no vagrant@192.168.56.83 \
                                "trivy image debian3:5000/asp-template"
                            '''
                        }
                    }
                }
                stage('Vulnerability scan') {
                    steps {
                        sshagent(['ssh-remote']) {
                            sh '''
                                ssh -o StrictHostKeyChecking=no vagrant@192.168.56.83 \
                                "docker run --rm -v $(pwd):/zap/wrk/:rw -t softwaresecurityproject/zap-stable zap-api-scan.py -I -t http://192.168.56.83:10000/swagger/v1/swagger.json -f openapi"
                            '''
                        }
                    }
                }
            }
        }
        stage('Deploy production') {
            when {
                expression { params.CD == "Production" || params.CD == "PassProduction" || params.Auto }
            }
            steps {
                sshagent(['ssh-remote']) {
                    sh '''
                        ssh -o StrictHostKeyChecking=no vagrant@192.168.56.84 \
                        "export KUBECONFIG=/home/vagrant/.kube/config.yml 
                        kubectl rollout restart deployment/asp-template-deployment"
                    '''
                }
            }
        }
    }
    // post {
    //     always {
    //         echo "Clean up"
    //     }
    // }
}