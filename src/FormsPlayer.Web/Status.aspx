<%@ Page Language="C#" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Status</title>
</head>
<body>
    <div>
        <p>
        FormsPlayer: <%= Xamarin.Forms.Player.ThisAssembly.Version %>
        </p>
        <p>
        SignalR: <%= typeof(Microsoft.AspNet.SignalR.Hub).Assembly.GetName().Version %>
        </p>
    </div>
</body>
</html>
