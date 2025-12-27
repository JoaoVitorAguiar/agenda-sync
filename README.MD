# **AgendaSync** 📅

**Web app to create and manage events synchronized with Google Calendar via OAuth2.**

<div style="display: flex; gap: 10px; flex-wrap: wrap; margin-top:10px;">
  <img height="35" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/csharp/csharp-original.svg" title=".NET C#" />
  <img height="35" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/dotnetcore/dotnetcore-original.svg" title=".NET 10" />
  <img height="35" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/react/react-original.svg" title="React" />
  <img height="35" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/vitejs/vitejs-original.svg" title="Vite" />
  <img height="35" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/docker/docker-original.svg" title="Docker" />
  <img height="35" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/postgresql/postgresql-original.svg" title="PostgreSQL" />
</div>

---

## 📌 **Features**

* Google Login via OAuth2 (Auth Code Flow)
* Create, update and list events from Google Calendar
* JWT Authentication with .NET
* Containerized full stack (PostgreSQL + API + Web App)
* Automatic EF Core migrations on startup
* `.env` configuration support (no secrets in the repository)

---

## 🐳 **Running with Docker**

### 1️⃣ Create your `.env` file

Copy the example and configure values:

```sh
cp .env.example .env
```

---

### 2️⃣ Build & run containers

```sh
docker compose --env-file .env up --build
```

---

### 📍 Accessing the app

| Service    | URL                                            |
| ---------- | ---------------------------------------------- |
| Frontend   | [http://localhost:3000](http://localhost:3000) |
| API        | [http://localhost:5069](http://localhost:5069) |
| PostgreSQL | localhost:5432                                 |

---

### 🔧 Useful commands

| Action                  | Command                      |
| ----------------------- | ---------------------------- |
| Stop containers         | `docker compose down`        |
| Stop + remove DB volume | `docker compose down -v`     |
| Show logs from API      | `docker compose logs -f api` |


---

## 🔧 Folder Structure

```
AgendaSync/
├── api/              # .NET backend
├── app/              # React + Vite frontend
├── docker-compose.yml
├── .env.example
└── README.md
```

---

## 🔐 Security Notes

* ⚠️ Never commit `.env` or real secrets to Git
* Use a strong `JWT_SECRET` in production
* Configure Google OAuth correctly in Google Cloud Console:

  * **Authorized JavaScript origins:** frontend URL (localhost or production domain)
  * **Authorized redirect URIs:** your API OAuth callback

---

## 🐛 Troubleshooting

| Issue                                      | Fix                                               |
| ------------------------------------------ | ------------------------------------------------- |
| Migration race conditions                  | Delete volume + restart: `docker compose down -v` |
| Google OAuth error `redirect_uri_mismatch` | Update Redirect URL in Google Cloud Console       |
| Frontend `.env` not applying               | Rebuild: `docker compose build web_app`           |

