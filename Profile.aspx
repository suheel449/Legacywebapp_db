<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Profile.aspx.cs" Inherits="ProfilePage" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Profile — Legacy Auth</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <article class="card">
    <h2>Account details</h2>
    <p class="subtitle">Loaded from the Users table in LegacyBizDb.</p>

    <dl class="profile">
      <div>
        <dt>Name</dt>
        <dd><asp:Literal ID="litName" runat="server" /></dd>
      </div>
      <div>
        <dt>Email</dt>
        <dd><asp:Literal ID="litEmail" runat="server" /></dd>
      </div>
      <div>
        <dt>Role</dt>
        <dd><asp:Literal ID="litRole" runat="server" /></dd>
      </div>
      <div>
        <dt>Status</dt>
        <dd><asp:Literal ID="litStatus" runat="server" /></dd>
      </div>
    </dl>
  </article>
</asp:Content>
