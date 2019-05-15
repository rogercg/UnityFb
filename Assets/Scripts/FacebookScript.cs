using System.Collections.Generic;
using UnityEngine;

// Importaciones 
using Facebook.Unity;
using UnityEngine.UI;

public class FacebookScript : MonoBehaviour
{

    public Text FriendsText;

    private void Awake()
    {   
        // Si FB no esta inicializado
        if (!FB.IsInitialized)
        {   
            // Lo inicializo
            FB.Init(() =>
            {   // Si ya esta inicializado
                if (FB.IsInitialized)
                    // Lo activo
                    FB.ActivateApp();
                else
                    // No se puede inicializar FB
                    Debug.LogError("Couldn't initialize");
            },
            // El juego es mostrado ?
            isGameShown =>
            {   
                // Si el juego es mostrado
                if (!isGameShown)
                    Time.timeScale = 0;// Congelamos el juego
                else
                    Time.timeScale = 1;// No congelamos el juego
            });
        }
        else
            // Activamos FB
            FB.ActivateApp();
    }

    #region Login / Logout
    public void FacebookLogin()
    {   
        /*
        // ESPECIFICAMOS LOS PERMISOS AL LOGUEARNOS
        var permissions = new List<string>() { "public_profile", "email", "user_friends" };
        // INICIAMOS SESION CON LOS PERMISOS INDICADOS
        FB.LogInWithReadPermissions(permissions, result=>{
            Debug.Log("Login: " + result);
        });// se puede tener un segundo parametro de callback
        */
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    public void FacebookLogout()
    {
        // CIERRO SESION
        FB.LogOut();
    }
    #endregion

    public void FacebookShare()
    {   
        // COMPARTO UN LINK  con un titulo, un mensaje y una imagen
        /*FB.ShareLink(new System.Uri("https://textilperu.com.pe"), "Titulo del mensaje",
            "Mensaje a compartir, se recomienda que no sea extenso",
            new System.Uri("https://resocoder.com/wp-content/uploads/2017/01/logoRound512.png"));// Imagen 
        */
        FB.ShareLink(
            new System.Uri("https://developers.facebook.com/"),
            callback: ShareCallback
        );
    }

    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !System.String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!System.String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
        }
    }

    #region Inviting
    public void FacebookGameRequest()
    {
        // COMPARTO UN ESTE APP  con un titulo y un mensaje
        //FB.AppRequest("Hey! Come and play this awesome game!", title: "Reso Coder Tutorial");
        FB.AppRequest("Come play this great game!",null, null, null, null, null, null,delegate (IAppRequestResult result) {
            Debug.Log(result.RawResult);
        }
);
    }

    public void FacebookInvite()
    {
        // INVITO A MIS AMIGOS EL JUEGO DE LA TIENDA CORRESPONDIENTE
        /*FB.Mobile.AppInvite(new System.Uri("https://play.google.com/store/apps/details?id=com.rogercg.snakerogercg"));*/
        var tutParams = new Dictionary<string, object>();
        tutParams[AppEventParameterName.ContentID] = "tutorial_step_1";
        tutParams[AppEventParameterName.Description] = "First step in the tutorial, clicking the first button!";
        tutParams[AppEventParameterName.Success] = "1";

        FB.LogAppEvent(
            AppEventName.CompletedTutorial,
            parameters: tutParams
        );
    }
    #endregion

    public void GetFriendsPlayingThisGame()
    {   
        // QUERY para obtener mis amigos
        string query = "/me/friends";
        // Mediante HTTP obtengo mi lista de amifos
        FB.API(query, HttpMethod.GET, result =>
        {
            Debug.Log("Amigos: " + result);
            // Deserializo y capturo de un formato JSON que sera devuelto por FB
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            // Obtengo la lista por objetos dictionary
            var friendsList = (List<object>)dictionary["data"];
            // Limpio el string y el cambpo de texto
            FriendsText.text = string.Empty;
            // Recorro la lista de amigos
            foreach (var dict in friendsList)
                // Asigno los amigos a el campo de texto(por nombre y por amigo capturado)
                FriendsText.text += ((Dictionary<string, object>)dict)["name"];
                Debug.Log("Recorrido de amigos: " + FriendsText.text);
        });
    }

}
