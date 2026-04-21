# NameGen
TO-DO: Live Custom Domain

## Overview
**NameGen** is a hybrid name generator REST API built with C# / .NET 8 and deployed on AWS. Generates human names, fictional character names, and gaming usernames using a combination of real datasets, rule-based syllable chaining, and AI enhancement.

## Live API

Base URL (TO-DO): `http://namegen-prod.eba-mt2b3zga.us-east-1.elasticbeanstalk.com`  
Swagger UI (TO-DO): `http://namegen-prod.eba-mt2b3zga.us-east-1.elasticbeanstalk.com/swagger`

## Features

What it does:

- Generates *human names* from the SSA Baby Names and US Census Surname datasets — filter by gender, length, character patterns, and popularity weighting
- Generates *fictional character names* using a rule-based syllable chaining engine with style presets including elvish, nordic, and villainous
- Generates *gaming usernames* with style presets including sweaty, clean, retro, and fantasy
- Save favorites — bookmark generated names for later reference
- Generation history — automatically logs every generation request for review
- AI enhancement — optionally passes a name through OpenAI to generate lore blurbs, style variations, or cultural origins

Tech Stack:

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
