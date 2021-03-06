terraform {
  required_providers {
    aws = {
      source = "hashicorp/aws"
    }
    null = {
      source = "hashicorp/null"
    }
  }
  required_version = ">= 0.13"
}

provider "aws" {
  access_key                  = "mock_access_key"
  region                      = "us-east-1"
  s3_force_path_style         = true
  secret_key                  = "mock_secret_key"
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true

  endpoints {
    dynamodb       = "http://localhost:4566"
  }
}