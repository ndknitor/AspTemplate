// Generic Webhook Trigger
// SSH Agent
// def username = "vagrant"
// def devHost = "192.168.56.110"
// def stageHost = "192.168.56.110"
// def prodHost = "192.168.56.84"
pipeline {
    agent any
    environment {
        REGISTRY = "utility.ndkn.local"
        IMAGE_NAME = "utility.ndkn.local/ndkn/asp-template"

        ARGOCD_SERVER= "192.168.121.104"
        ARGOCD_APP_NAME = "asp-template"
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
        stage('Setup') {
            steps {
                sh 'export DOTNET_CLI_TELEMETRY_OPTOUT=0'
            }
        }
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
        }
        stage('Build image') {
            when {
                expression { params.CD == "Development" }
            }
            steps {
                    sh 'docker build -t ${IMAGE_NAME} .'
                }
            }
        stage('Push image to registry')
        {
            when {
                expression { params.CD == "Development" }
            }
            steps {
                script{
                    withCredentials([usernamePassword(credentialsId: 'registry_credentials', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                        sh 'docker login utility.ndkn.local -u="${DOCKER_USERNAME}" -p="${DOCKER_PASSWORD}"'
                        sh 'docker push ${IMAGE_NAME}'
                        sh 'docker image prune -f'
                    }
                }
            }
        }
        // stage('Clone Ops Repository') {
        //     steps {
        //         script {
        //             // Clone the source repository
        //             git credentialsId: "GitNDKN", url: "https://github.com/ndknitor/asp-template-gitops"
        //         }
        //     }
        // }
        stage('Trigger ArgoCD sync for developent environment') {
            script {
                withCredentials([string(credentialsId: 'argocd_ds_token', variable: 'ARGOCD_TOKEN')])
                steps {
                    sh """curl -H "Authorization': "Bearer ${ARGOCD_TOKEN}" -x POST  "https://${ARGOCD_SERVER}/api/v1/applications/${ARGOCD_APP_NAME}/sync" """
                }
            }
        }
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