import api from "./api";

export async function loginWithGoogle(code: string) {
    await api.post("/auth/google/login", { code }, { withCredentials: true });
    window.location.href = "/dashboard";
}


export function logout() {
    document.cookie = "agenda_token=; Max-Age=0";
    window.location.href = "/login";
}

export async function checkSession(): Promise<boolean> {
    try {
        await api.get("/auth/check", { withCredentials: true });
        return true;
    } catch {
        return false;
    }
}