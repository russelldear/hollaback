resource "aws_dynamodb_table" "posted-items" {
  name           = "posted-items"
  billing_mode   = "PROVISIONED"
  read_capacity  = 2
  write_capacity = 2
  hash_key       = "Id"

  attribute {
    name = "Id"
    type = "S"
  }
}