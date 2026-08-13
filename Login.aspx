<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="LoginPage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Sign in — Legacy Auth</title>
  <link rel="stylesheet" href="Styles/Site.css" />
</head>
<body>
  <form id="form1" runat="server">
    <div class="page">
      <aside class="brand-panel">
        <p class="brand">Legacy Auth</p>
        <p class="tagline">ASP.NET Web Forms login. No database backend.</p>
      </aside>

      <main class="form-panel">
        <h1>Sign in</h1>
        <p class="subtitle">Use the account you registered in this app.</p>

        <div class="fields">
          <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Email" />
          <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="input" />

          <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" Text="Password" />
          <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input" />

          <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />

          <asp:Button ID="btnLogin" runat="server" Text="Sign in" CssClass="btn primary" OnClick="btnLogin_Click" />
        </div>

        <p class="switch">
          No account?
          <a href="Register.aspx">Create one</a>
        </p>
      </main>
    </div>
  </form>
</body>
</html>
