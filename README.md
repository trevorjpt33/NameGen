# NameGen
TO-DO: Live Custom Domain

## Overview
A hybrid name generator REST API built with C# / .NET 8 and deployed on AWS. Generates human names, fictional character names, and gaming usernames using a combination of real datasets, rule-based syllable chaining, and AI enhancement.

## Live API

Base URL: `http://namegen-prod.eba-mt2b3zga.us-east-1.elasticbeanstalk.com`  
Swagger UI: `http://namegen-prod.eba-mt2b3zga.us-east-1.elasticbeanstalk.com/swagger`

## Tech Stack

- Language / Runtime: C# / .NET 8
- Framework: ASP.NET Core Web API
- Frontend: Blazor WebAssembly
- Database: Amazon RDS (PostgreSQL 16)
- ORM: Entity Framework Core
- Validation: FluentValidation
- Logging: Serilog → Amazon CloudWatch
- Cloud: AWS (Elastic Beanstalk, RDS, S3, CloudFront, ECR, SSM, CloudWatch)
- IaC: AWS CloudFormation
- Containers: Docker + Minikube
- Orchestration: Kubernetes (Amazon EKS)
- CI/CD: GitHub Actions
- Testing: xUnit + Moq

## License

MIT — see [LICENSE](LICENSE)
