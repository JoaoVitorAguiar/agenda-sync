import { useGoogleLogin } from "@react-oauth/google";
import axios from "axios";

export function Login() {
    const login = useGoogleLogin({
        flow: "auth-code",

        scope: [
            "openid",
            "profile",
            "email",
            "https://www.googleapis.com/auth/calendar",
        ].join(" "),

        onSuccess: async (codeResponse) => {
            try {

                // TODO
                // console.log("Authorization code:", codeResponse.code);

                // const response = await axios.post(
                //     "http://localhost:5106/auth/google",
                //     { code: codeResponse.code }
                // );

                // const { accessToken } = response.data;

                // localStorage.setItem("accessToken", accessToken);

                // window.location.href = "/dashboard";
            } catch (err) {
                console.error("Erro ao autenticar:", err);
                alert("Falha ao autenticar com Google");
            }
        },

        onError: () => {
            alert("Login com Google cancelado");
        },
    });

    return (
        <div style={styles.container}>
            <h1 style={styles.title}>AgendaSync</h1>
            <p style={styles.subtitle}>
                Conecte sua conta Google para acessar sua agenda
            </p>

            <button onClick={() => login()}>
                Conectar Google Calendar
            </button>
        </div>
    );
}

const styles = {
    container: {
        height: "100vh",
        display: "flex",
        flexDirection: "column" as const,
        justifyContent: "center",
        alignItems: "center",
        gap: "1.5rem",
    },
    title: {
        fontSize: "2.5rem",
        fontWeight: 700,
    },
    subtitle: {
        color: "#666",
        maxWidth: 400,
        textAlign: "center" as const,
    },
};
