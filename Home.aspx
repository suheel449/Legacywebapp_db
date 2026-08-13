<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="HomePage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Home — Legacy Auth</title>
  <link rel="stylesheet" href="Styles/Site.css" />
</head>
<body>
  <form id="form1" runat="server">
    <div class="page home">
      <header class="topbar">
        <p class="brand compact">Legacy Auth</p>
        <asp:Button ID="btnLogout" runat="server" Text="Sign out" CssClass="btn ghost" OnClick="btnLogout_Click" />
      </header>

      <main class="home-panel">
        <h1><asp:Literal ID="litWelcome" runat="server" /></h1>
        <p class="subtitle">You are signed in. Session is in-memory only (no database).</p>
        <dl class="profile">
          <div>
            <dt>Name</dt>
            <dd><asp:Literal ID="litName" runat="server" /></dd>
          </div>
          <div>
            <dt>Email</dt>
            <dd><asp:Literal ID="litEmail" runat="server" /></dd>
          </div>
        </dl>
      </main>
    </div>
  </form>
</body>
</html>
