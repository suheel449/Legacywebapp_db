<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="RegisterPage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Register — Legacy Auth</title>
  <link rel="stylesheet" href="Styles/Site.css" />
</head>
<body>
  <form id="form1" runat="server">
    <div class="page">
      <aside class="brand-panel">
        <p class="brand">Legacy Auth</p>
        <p class="tagline">Register with ASPX pages. Accounts saved to SQL Server.</p>
      </aside>

      <main class="form-panel">
        <h1>Create account</h1>
        <p class="subtitle">New accounts are inserted into the Users table.</p>

        <div class="fields">
          <asp:Label ID="lblName" runat="server" AssociatedControlID="txtName" Text="Full name" />
          <asp:TextBox ID="txtName" runat="server" CssClass="input" />

          <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Email" />
          <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="input" />

          <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" Text="Password" />
          <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input" />

          <asp:Label ID="lblConfirm" runat="server" AssociatedControlID="txtConfirm" Text="Confirm password" />
          <asp:TextBox ID="txtConfirm" runat="server" TextMode="Password" CssClass="input" />

          <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />

          <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn primary" OnClick="btnRegister_Click" />
        </div>

        <p class="switch">
          Already registered?
          <a href="Login.aspx">Sign in</a>
        </p>
      </main>
    </div>
  </form>
</body>
</html>
