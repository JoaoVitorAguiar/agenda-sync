import { useGoogleLogin } from "@react-oauth/google";
import { useState } from "react";
import { loginWithGoogle } from "../../services/authService";
import "./styles.css";

export function Login() {
    const [loading, setLoading] = useState(false);

    const login = useGoogleLogin({
        flow: "auth-code",
        scope: [
            "openid",
            "profile",
            "email",
            "https://www.googleapis.com/auth/calendar",
        ].join(" "),

        onSuccess: async ({ code }) => {
            setLoading(true);
            try {
                await loginWithGoogle(code);
            } catch (err) {
                alert("Error authenticating with Google");
                console.error(err);
            } finally {
                setLoading(false);
            }
        },

        onError: () => alert("Login canceled"),
    });

    return (
        <div className="login-container">
            <h1 className="login-title">AgendaSync</h1>
            <p className="login-subtitle">
                Connect your Google account to access your calendar&nbsp;📅
            </p>

            <button className="google-btn" onClick={() => login()} disabled={loading}>
                <img
                    src="https://www.svgrepo.com/show/475656/google-color.svg"
                    alt="Google"
                    className="google-icon"
                />
                {loading ? "Connecting..." : "Sign in with Google"}
            </button>

            <span className="login-footer">
                Secure authorization via OAuth2 / Google Identity
            </span>
        </div>
    );
}
