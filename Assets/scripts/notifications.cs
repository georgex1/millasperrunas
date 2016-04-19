using UnityEngine;
using System.Collections;

//Add these Namespaces
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;
using VoxelBusters.AssetStoreProductUtility.Demo;

public class notifications : MonoBehaviour {

	private MainController GMS;
	private bool isInvitacion = false;
	private bool isChat = false;

	[SerializeField, EnumMaskField(typeof(NotificationType))]
	private NotificationType	m_notificationType;
	
	
	void Start()
	{
		Debug.Log ("start notif controller");
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		string config_alerts = PlayerPrefs.GetString ("config_alerts");
		Debug.Log ("notificaciones activadas: " + config_alerts);
		NPBinding.NotificationService.RegisterNotificationTypes (m_notificationType);
		//if (config_alerts == "true") {
			NPBinding.NotificationService.RegisterForRemoteNotifications ();
		//}
	}

	public void disableNotifs(){
		NPBinding.NotificationService.UnregisterForRemoteNotifications ();
	}

	public void enableNotifs(){
		NPBinding.NotificationService.RegisterForRemoteNotifications ();
	}

	void OnEnable ()
	{
		Debug.Log ("enable notif");
		// Register RemoteNotificated related callbacks
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	+= DidFinishRegisterForRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			+= DidReceiveRemoteNotificationEvent;
		
		//Add below for local notification
		//NotificationService.DidReceiveLocalNotificationEvent 			+= DidReceiveLocalNotificationEvent;
		
	}
	
	void OnDisable ()
	{
		// Un-Register from callbacks
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	-= DidFinishRegisterForRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			-= DidReceiveRemoteNotificationEvent;
		
		//Add below for local notification
		//NotificationService.DidReceiveLocalNotificationEvent 			-= DidReceiveLocalNotificationEvent;
		
	}
	
	
	#region API Callbacks
	
	private void DidReceiveLocalNotificationEvent (CrossPlatformNotification _notification)
	{
		Debug.Log("Received DidReceiveLocalNotificationEvent : " + _notification.ToString());
	}
	
	private void DidReceiveRemoteNotificationEvent (CrossPlatformNotification _notification)
	{ //CALLBACK CUANDO SE ABRE LA APP DESDE LA NOTIF O LA APP ESTA ABIERTA

		//bajar perros si es una notificacion de paseo
		GMS.download_perros ();
		GMS.call_updates ("notificaciones");

		/*if (_notification.ToString ().Contains ("Chat")) {
			isChat = true;
		} else {
			isChat = false;
		}*/

		IDictionary _userInfo 			= _notification.UserInfo;
		if (_userInfo != null) {

			if((string)_userInfo["notifType"] == "chat"){
				if( Application.loadedLevelName != "chat" ){

					string[] m_buttons = new string[] { "Cerrar", "Ver" };
					NPBinding.UI.ShowAlertDialogWithMultipleButtons ("Mensaje de " + (string)_userInfo["usuario_nombre"], _notification.AlertBody, m_buttons, (string _buttonPressed)=>{
						if(_buttonPressed == "Ver"){

							GMS.db.OpenDB("millasperrunas.db");
							ArrayList result = GMS.db.BasicQueryArray ( GMS.perro.queryPerro( (string)_userInfo["perros_id"], GMS.userData.id) );
							GMS.db.CloseDB();
							
							foreach (string[] row_ in result) {
								Debug.Log(row_[1]);
								GMS.perro.populatePerro (row_);
							}
							GMS.paseoPerroId = GMS.perro.id;

							PlayerPrefs.SetString("chatUserId", (string)_userInfo["amigos_usuarios_id"]);

							Application.LoadLevel("chat");
						}
					});
				}
			}

			else if((string)_userInfo["notifType"] == "invitacion"){
				string[] m_buttons = new string[] { "Cerrar", "Ver" };
				NPBinding.UI.ShowAlertDialogWithMultipleButtons ("Invitación para pasear a " + (string)_userInfo["perro_nombre"], _notification.AlertBody, m_buttons, (string _buttonPressed)=>{
					if(_buttonPressed == "Ver"){
						//GMS.notificacionId = (string)_userInfo["notificacion_id"];
						Application.LoadLevel ("notificaciones");
					}
				});
			}
			else{
				string[] m_buttons = new string[] { "Cerrar", "Ver" };
				NPBinding.UI.ShowAlertDialogWithMultipleButtons ("Tienes una nueva notificación", _notification.AlertBody, m_buttons, (string _buttonPressed)=>{
					if(_buttonPressed == "Ver"){
						//GMS.notificacionId = (string)_userInfo["notificacion_id"];
						Application.LoadLevel ("notificaciones");
					}
				});
			}

			/*foreach (string _key in _userInfo.Keys){
				if(_key == "alertMgs"){
				}
				//_userInfoDetails	+= _key + " : " + _userInfo[_key] + "\n";
			}*/

			/*NPBinding.UI.ShowAlertDialogWithSingleButton("Mensaje en DidReceiveRemoteNotificationEvent", _userInfo[_key].ToString(), "OK", (string _buttonPressed)=>{
						AddNewResult("Alert dialog closed");
						AppendResult("ButtonPressed=" + _buttonPressed);
					});*/
		}

		//_notification.UserInfo

		//_notification.AndroidProperties.TickerText
		/*if (!isChat) {
			Debug.Log ("Received DidReceiveRemoteNotificationEvent : " + _notification.ToString ());
			string[] m_buttons = new string[] { "Ver" };
			NPBinding.UI.ShowAlertDialogWithMultipleButtons ("Alerta!", _notification.AlertBody, m_buttons, MultipleButtonsAlertClosed); 
		}*/
	}

	private void processRecivedNotif(CrossPlatformNotification _notification){
	}

	private void SingleButtonsAlertClosed (string _buttonPressed){
	}

	/*private void MultipleButtonsAlertClosed (string _buttonPressed)
	{
		if (_buttonPressed == "Ver") {
			GMS.call_updates ("notificaciones");
			if(isInvitacion){
				Application.LoadLevel ("home");
			}else if(isChat){
				Application.LoadLevel ("ranking");
			}else{
				Application.LoadLevel ("notificaciones");
			}
		}
	}*/

	private void DidLaunchWithRemoteNotificationEvent (CrossPlatformNotification _notification)
	{
		GMS.call_updates ("notificaciones");
		/*if (_notification.ToString ().Contains ("Chat")) {
			Application.LoadLevel ("ranking");
		}else{
			Application.LoadLevel ("notificaciones");
		}*/


		/*IDictionary _userInfo 			= _notification.UserInfo;
		if (_userInfo != null) {
			foreach (string _key in _userInfo.Keys){
				if(_key == "alertMgs"){
					NPBinding.UI.ShowAlertDialogWithSingleButton ("Mensaje.", _userInfo[_key].ToString(), "OK", SingleButtonsAlertClosed); 
				}
				//_userInfoDetails	+= _key + " : " + _userInfo[_key] + "\n";
			}
		}*/
	}
	
	private void DidFinishRegisterForRemoteNotificationEvent (string _deviceToken, string _error)
	{
		if(string.IsNullOrEmpty(_error))
		{
			Debug.Log("Device Token : " + _deviceToken);
			GMS.userData.reg_id = _deviceToken;


#if !UNITY_EDITOR
#if UNITY_ANDROID
			GMS.userData.plataforma = "Android";
#else
			GMS.userData.plataforma = "IOS";
#endif
#endif
			StartCoroutine(GMS.updateRegId());
		}
		else
		{
			Debug.Log("Error in registering for remote notifications : " + _deviceToken);
		}
	}
	
	#endregion
}
