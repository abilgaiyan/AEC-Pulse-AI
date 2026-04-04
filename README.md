# AEC-Pulse AI: Intelligent ERP Orchestration Agent

A high-performance .NET 9 & React solution designed to transform static Project-Based ERP data into actionable business intelligence using Generative AI.

## 🚀 The Vision
Most Professional Services Automation (PSA) tools are "data graveyards." AEC-Pulse AI uses RAG (Retrieval-Augmented Generation) and Agentic Workflows to allow project managers and executives to query their financial health, resource capacity, and project risks using natural language.

## 🛠️ Tech Stack
- **Backend:** .NET 9 (Minimal APIs, C# 13)
- **AI Orchestration:** Semantic Kernel (Microsoft)
- **Frontend:** React 19 + TypeScript + Tailwind CSS
- **Database:** SQL Server (Relational) + Pinecone (Vector Store)
- **Patterns:** Clean Architecture, CQRS, Repository Pattern

## ✨ Key Features
- **Natural Language Financial Queries:** "Which projects are currently over budget but under 50% completion?" 
- **Predictive Resource Insights:** Analyzes historical timesheet data to predict employee burnout and billability gaps.
- **RAG-Powered Policy Bot:** Instant answers to company billing and absence policies grounded in internal documentation.
- **Automated Data Enrichment:** AI-driven validation for CRM firm records to ensure data integrity.

## 🏗️ Architecture Summary
AEC-Pulse AI utilizes a **Mediator Pattern** to decouple business logic. The AI layer acts as a "Reasoning Engine" that sits between the user and the SQL database, ensuring that all data retrieval is grounded and secure.

## 🚦 Getting Started
1. Clone the repo
2. Run `docker-compose up`
3. Access the dashboard at `localhost:3000`