pipeline {
    agent any
    stages {
        // stage('Clone repository') {
        //     steps {
        //          git branch: 'main', credentialsId: 'Ndkn', url: 'https://github.com/ndknitor/JenkinsImpact'
        //     }
        // }
        stage('Build') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --no-restore'
            }
        }
        // stage('Test') {
        //     steps {
        //         sh 'dotnet test'
        //     }
        // }
        // stage('Run') {
        //     steps {
        //         sh 'dotnet run --project JenkinsImpact'
        //     }
        // }
        stage('Deploy development') {
            when {
                expression { params.CD == "Development" }
            }
            steps {
                sshagent(credentials: ['ssh_credential']) {
                    sh '''
                        ssh vagrant@192.168.56.82 \
                        "cd AspTemplate &&
                        git pull &&
                        docker build -t asp-template . &&
                        docker stop asp-template &&
                        docker run --name asp-template --rm -d -p 5000:8080 asp-template &&
                        docker image prune -f"
                    '''
                }
            }
        }
        // stage('Deploy staging') {
        //     when {
        //         expression { params.CD == "Staging" }
        //     }
        //     steps {
        //         echo 'Deploy staging'
        //     }
        // }
        // stage('Deploy production') {
        //     when {
        //         expression { params.CD == "Production" }
        //     }
        //     steps {
        //         echo 'Deploy production'
        //     }
        // }
    }
    post {
        always {
            echo "Clean up"
        }
    }
}