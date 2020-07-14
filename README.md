# README #

Servicios Api Rest que son consumidos por el sitio web institucional.

### Acerca del repositorio ###

* PaginaWeb-API 
* Versión 1.0.0
* [Clonar](http://192.168.57.181/desarrollo/PaginaWeb-API.git)

### Estructura ###

--APS.Web.Api         -> Directorio principal

----Controllers       -> Directorio con los controladores del API (Capa de negocio).

----DB                -> Directorio que contiene las clases con los métodos que se conectan a las bases de datos (Capa de acceso a datos).

----Models            -> Directorio con las clases que definen las estructuras de datos (POCOs).

----Utils             -> Directorio con las clases que contienen funciones utilitarios.

----appsettings.json  -> Archivo que Incluye y define las variables globales

----Program.cs        -> Archivo que inicia la ejecución del API.

----Startup.cs        -> Archivo que inicia las configuraciones globales.



### Configuración ###

* appsettings.json -> Incluye y define las variables globales

```
#!javascript
{
	"Logging": {
		"IncludeScopes": false,
		"LogLevel": {
			"Default": "Warning"
		}
	},

	"ConnectionStrings": {
		"DocsConnectionString": "Server=DESARROLLO;Database=dbWeb;User Id=sqlapp_webresol_editor;Password=pkjs2387kxdfer3;",
		"ReclamosConnectionString": "Server=DESARROLLO;Database=SIRESERE;User Id=svc_srvdpc_siresere_dev;Password=svc_srvdpc_siresere_dev;",
		"SegurosConnectionString": "Server=DESARROLLO;Database=dbSeguros;User Id=app_DataApi;Password=}dm%9Vp[Ejk(^tmr;",
		"SipofConnectionString": "Server=DESARROLLO;Database=dbSIPOF;User Id=usrsipof;Password=asdfg"
	},

	"ResolucionesPath": "http://aplicaciones.aps.gob.bo/ResolucionesABM/Datos/Resoluciones/",
	"CircularesPath": "http://aplicaciones.aps.gob.bo/ResolucionesABM/Datos/Circulares/",
	"Reclamo": {
		"EmailFrom": "contactenos@aps.gob.bo",
		"EmailFromUser": "APS reclamos web",
		"EmailTo": "contactenos@aps.gob.bo",
		"EmailToUser": "Alvaro",
		"MailServer": "mail.aps.gob.bo",
		"MailPort": "25",
		"MailUseSsl": "false",
		"MailUser": "emailResoluciones",
		"MailUserPassword": "PASSWORD",
		"MailDefaultSubject": "Reclamo vía web"
	},
	"WebProxy": {
		"Url": "http://192.168.58.79:9090",
		"Enabled": "true",
		"User": "jmamani",
		"Password": "PASSWORD"
	},
	"GoogleApis": {
		"Recaptcha": {
			"SiteKey": "6LfumzUUAAAAAArgQOK52eFc0svPnAE9kZ0mspWD",
			"SecretKey": "6LfumzUUAAAAAFzNhvYIDZhXf1JM1r-v6B0UYSFV"
		}
	}
}
```

### A quien contactar ###

* Jhenrry Alvaro Mamani Javier
* Dirección de Sistemas