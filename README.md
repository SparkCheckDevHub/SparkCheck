# 🧭 VibeCheck Project Structure Guide

Welcome to the VibeCheck project! This guide explains how the solution is organized so you can confidently contribute without breaking anything.

---

## 📁 wwwroot/

The **public web root** for static assets.

- `css/` – Custom styles (`app.css`, etc.)
- `img/` – App images, logos, icons

---

## 🧩 Shared/

Reusable UI components **organized by feature**.

- `Layout/`
  - `MainLayout.razor` – App shell (header, sidebar)
  - `NavMenu.razor` – Navigation bar

- Other folders like `Chat/`, `Match/` will hold feature-specific UI parts.

---

## 🧬 Data/

Database-related logic using **Entity Framework Core**.

- `AppDbContext.cs` – DB context with DbSets for Users, Matches, etc.
- `DbSeeder.cs` – (Optional) Seeds test data for local dev

---

## 💬 Hubs/

**SignalR real-time communication** using WebSockets.

- `ChatHub.cs` – Used for live chat messaging

---

## 🧠 Models/

All **data models (POCOs)** used across the app.

- `TUser.cs`, `Match.cs`, `ChatMessage.cs`, `Preferences.cs`
- Used in the DB, services, UI, and API calls

---

## 📄 Pages/

Blazor **routable screens**—each `.razor` file maps to a route.

- `Welcome.razor` – First-time screen
- `PhoneLogin.razor` – Login with phone number
- `CreateAccount.razor` – Sign-up form
- `MatchQueue.razor`, `Dashboard.razor` – Main app views

---

## 🛠 Services/

C# classes that contain **business logic** or backend communication.

- `AuthService.cs` – Login, auth logic
- `UserService.cs` – Fetching/updating user data
- `MatchService.cs` – Finding matches
- `ChatService.cs` – Managing chat messages/history

This keeps logic **separate from the UI**.

---

## 📦 Root-Level Files

| File                     | Purpose                                       |
|--------------------------|-----------------------------------------------|
| `.gitignore`             | Tells Git what to exclude (e.g., `/bin/`)     |
| `App.razor`              | Root component entry for Blazor               |
| `_Imports.razor`         | Global `@using` directives                    |
| `Program.cs`             | App startup and service registration          |
| `Routes.razor`           | Blazor routing configuration                  |
| `appsettings.json`       | Configs like connection strings, secrets      |
| `VibeCheck.sln`          | Visual Studio solution file                   |

---

## ✅ Why This Setup?

This **feature-first, clean architecture** helps:

- Keep logic separated and organized
- Avoid spaghetti `.razor` files
- Let multiple devs work without stepping on each other
- Make it easy to add new features or services

---

## 🚀 Getting Started for New Devs

1. Clone the repo:  
   `git clone https://github.com/VibeCheckDevHub/VibeCheck.git`

2. Open `VibeCheck.sln` in Visual Studio

3. Run the app from `Welcome.razor`

4. Add new components **in the appropriate feature folder**  
   (Don’t crowd the root!)

---

Welcome aboard! ❤️
