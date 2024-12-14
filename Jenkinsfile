// Generic Webhook Trigger
// SSH Agent
// def username = "vagrant"
// def devHost = "192.168.56.110"
// def stageHost = "192.168.56.110"
// def prodHost = "192.168.56.84"

pipeline {
    agent any
    stages {
        // stage('Clone repository') {
        //     when {
        //         expression { params.CD == "Development" }
        //     }
        //     steps {
        //         git branch: 'main', credentialsId: 'Ndkn', url: 'https://github.com/ndknitor/AspTemplate'
        //     }
        // }
        stage('Build') {
            when {
                expression { params.CD == "None" || params.CD == "Development" }
            }
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --no-restore'
            }
        }
        stage('Test') {
            when {
                expression { params.CD == "None" || params.CD == "Development" }
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
    //     stage('Deploy development') {
    //         when {
    //             expression { params.CD == "Development" }
    //         }
    //         steps {
    //             sshagent(['ssh-remote']) {
    //                 sh """
    //                     ssh -o StrictHostKeyChecking=no ${username}@${devHost} \
    //                     "cd AspTemplate 
    //                     git pull 
    //                     docker build -t ${devHost}:5000/asp-template:dev . 
    //                     docker stop asp-template-dev 
    //                     docker rm asp-template-dev
    //                     docker run --name asp-template-dev -e ASPNETCORE_ENVIRONMENT="Development" --restart=always -d -p 8080:8080 ${devHost}:5000/asp-template:dev 
    //                     docker push ${devHost}:5000/asp-template:dev
    //                     docker image prune -f"
    //                 """
    //             }
    //         }
            
    //     }
    //     stage('Deploy staging') {
    //         when {
    //             expression { params.CD == "Staging" || params.CD == "PassProduction" || params.Auto}
    //         }
    //         steps {
    //             sshagent(['ssh-remote']) {
    //                 sh """
    //                     ssh -o StrictHostKeyChecking=no ${username}@${stageHost} \
    //                     "docker pull ${devHost}:5000/asp-template:dev 
    //                     docker tag ${devHost}:5000/asp-template:dev ${devHost}:5000/asp-template 
    //                     docker stop asp-template-staging
    //                     docker rm asp-template-staging 
    //                     docker run --name asp-template-staging -e ASPNETCORE_ENVIRONMENT="Staging" --restart=always -d -p 10000:8080 ${devHost}:5000/asp-template 
    //                     docker push ${devHost}:5000/asp-template 
    //                     docker image prune -f "
    //                 """
    //             }
    //         }
    //     }
    //     stage('Deploy production') {
    //         when {
    //             expression { params.CD == "Production" || params.CD == "PassProduction" || params.Auto }
    //         }
    //         steps {
    //             sshagent(['ssh-remote']) {
    //                 sh """
    //                     ssh -o StrictHostKeyChecking=no ${username}@${prodHost} \
    //                     "export KUBECONFIG=/home/${username}/.kube/config.yml 
    //                     kubectl rollout restart deployment/asp-template-deployment"
    //                 """
    //             }
    //         }
    //     }
    //     stage('Scan') {
    //         when {
    //             expression { params.CD == "Staging" || params.CD == "PassProduction"}
    //         }
    //         parallel {
    //             stage('Image scan') {
    //                 steps {
    //                     sshagent(['ssh-remote']) {
    //                         sh """
    //                             ssh -o StrictHostKeyChecking=no ${username}@${devHost} \
    //                             "trivy image ${devHost}:5000/asp-template"
    //                         """
    //                     }
    //                 }
    //             }
    //             stage('Vulnerability scan') {
    //                 steps {
    //                     sshagent(['ssh-remote']) {
    //                         sh """
    //                             ssh -o StrictHostKeyChecking=no ${username}@${devHost} \
    //                             '
    //                                 docker run --rm -t softwaresecurityproject/zap-stable zap-api-scan.py -I -t http://${devHost}:10000/swagger/v1/swagger.json -f openapi \
    //                                 -z "-config replacer.full_list(0).description=auth1 \
    //                                 -config replacer.full_list(0).enabled=true \
    //                                 -config replacer.full_list(0).matchtype=REQ_HEADER \
    //                                 -config replacer.full_list(0).matchstr=Authorization \
    //                                 -config replacer.full_list(0).regex=false \
    //                                 -config replacer.full_list(0).replacement=Bearer\\ \${asp-template-user-jwt}"
    //                             '
    //                         """
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }
    // post {
    //     always {
    //         echo "Clean up"
    //     }
    }
}
