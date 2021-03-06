cd localstack

docker-compose down
SERVICES=dynamodb docker-compose up -d

cd ../terraform

terraform init && terraform plan -out="infraplan"
terraform apply -auto-approve infraplan

cd ..