// Generic Webhook Trigger
// SSH Agent
pipeline {
    agent any
    environment {
        REGISTRY = "utility.ndkn.local"
        IMAGE_NAME = "utility.ndkn.local/ndkn/asp-template"

        ARGOCD_SERVER= "192.168.121.104:30892"
        ARGOCD_APP_NAME = "asp-template"
        ARGOCD_NAMESPACE = "asp-template"
        ARGOCD_RESOURCE_NAME_DEVELOPMENT = "asp-template-deployment-development"
        ARGOCD_RESOURCE_NAME_STAGING = "asp-template-deployment-staging"
    }
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
                expression { params.CD == "None" || params.CD == "Development" || params.Auto }
            }
            steps {
                sh 'export DOTNET_CLI_TELEMETRY_OPTOUT=0'
                sh 'dotnet restore'
                sh 'dotnet build --no-restore'
            }
        }
        stage('Test') {
            when {
                expression { params.CD == "None" || params.CD == "Development" || params.Auto}
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
        }
    }
}

        

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
