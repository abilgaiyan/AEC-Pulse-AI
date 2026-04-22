# 🚀 AEC-Pulse AI: Enterprise Agentic ERP Intelligence

**AEC-Pulse AI** is a next-generation "Reasoning Layer" for Architecture, Engineering, and Construction (AEC) firms. Built on the **Microsoft Agent Framework (MAF)** and **.NET 9**, it transforms static ERP data (like Deltek Vantagepoint) into actionable intelligence using Multi-Agent Orchestration.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![.NET 9](https://img.shields.io/badge/.NET-9.0-blue)
![React 19](https://img.shields.io/badge/React-19.0-61dafb)

---

## 🧠 The Architecture (2026 Standard)

This project utilizes **Clean Architecture** to decouple business logic from the AI orchestration layer. 

### **The AI Reasoning Stack**
* **Microsoft Foundry IQ:** Provides the Semantic Index (RAG) for internal project documents, billing policies, and labor laws.
* **Microsoft Agent Framework (MAF):** Orchestrates the conversation between specialized agents (Financial Analyst Agent vs. Compliance Agent).
* **Semantic Kernel:** Acts as the "Engine" for C# Function Calling, allowing the AI to execute real-time SQL calculations on project margins.

---

## ✨ Key Features

* **📈 Intelligent Margin Analysis:** Natural language querying of project profitability (e.g., *"Which Mumbai projects are at risk of 15% margin erosion?"*).
* **🤖 Multi-Agent Collaboration:** A "Financial Agent" queries the SQL DB while a "Compliance Agent" checks Foundry IQ for regulatory hits.
* **⚡ Real-time Streaming UI:** A React 19 dashboard featuring word-by-word streaming and "Agent Thought Process" transparency.
* **🛡️ Enterprise Governance:** Built-in PII masking and audit logging of all AI-human interactions for ERP compliance.

---

## 🛠️ Tech Stack

| Layer | Technology |
| :--- | :--- |
| **Frontend** | React 19, TypeScript, Tailwind CSS v4, Vite |
| **API / Backend** | .NET 9, ASP.NET Core, Minimal APIs |
| **AI Orchestration** | Microsoft Agent Framework (MAF), Semantic Kernel |
| **Intelligence** | Microsoft Foundry (GPT-4o / Llama 3.3), Foundry IQ |
| **Data** | SQL Server 2022/2026 (EF Core), Vector Memory |

---

## 🚀 Quick Start (Docker)

Ensure you have your **Microsoft Foundry** keys ready in an `.env` file, then run:

```bash
docker-compose up --build


## ⚙️ Configuration

### Backend (API)
1. Navigate to `src/AECPulse.API`.
2. Update `appsettings.Development.json` or use .NET User Secrets:
   ```bash
   dotnet user-secrets set "MicrosoftFoundry:ApiKey" "YOUR_KEY"