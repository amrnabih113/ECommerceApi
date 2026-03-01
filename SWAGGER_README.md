# 📚 ECommerce API - Offline Documentation

## 🚀 How to Use

### Option 1: Open Locally
Simply open `swagger-ui.html` in your web browser:
- Double-click `swagger-ui.html` 
- Or right-click → Open with → Your browser

### Option 2: Share with Team
1. Share these 2 files together:
   - `swagger-ui.html` 
   - `swagger.json`

2. Recipient opens `swagger-ui.html` in their browser

---

## 📖 What You Get

✅ **All API Endpoints**
- Request/Response examples
- Required parameters
- Data types
- Status codes

✅ **Authentication**
- JWT Bearer token support
- Security definitions

✅ **Interactive**
- Try-it-out functionality (if API is running)
- Search endpoints
- Filter by tags

---

## 🔄 How to Update

When your API changes:

### 1. Run Server
```bash
cd d:\Amr\C#_projects\ECommerce
dotnet run
```

### 2. Download New Swagger JSON
```powershell
Invoke-WebRequest -Uri "http://localhost:5206/swagger/v1/swagger.json" `
  -OutFile "D:\Amr\C#_projects\ECommerce\swagger.json"
```

### 3. Refresh Browser
Open `swagger-ui.html` and refresh (Ctrl+R)

---

## 📋 Files Included

```
📁 ECommerce API Docs
├── swagger-ui.html        ← Open this in browser
├── swagger.json           ← API specification (auto-generated)
└── README.md              ← This file
```

---

## 🔗 Features

| Feature | Support |
|---------|---------|
| View all endpoints | ✅ Yes |
| See request/response | ✅ Yes |
| Parameter descriptions | ✅ Yes |
| Try requests | ✅ Yes (if API running) |
| Authentication examples | ✅ Yes |
| Share with team | ✅ Yes |
| Works offline | ✅ Yes |

---

## ⚙️ System Requirements

- Modern web browser (Chrome, Firefox, Safari, Edge)
- No server installation needed
- No internet required (except for CDN resources)

---

**Generated:** March 1, 2026  
**API Version:** 1.0.0  
**Framework:** ASP.NET Core 10.0
