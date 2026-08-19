<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Profile.aspx.cs" Inherits="ProfilePage" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Profile — Legacy Auth</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <article class="card">
    <h2>Account details</h2>
    <p class="subtitle">Stored in session only — nothing is written to a database.</p>

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
        <dd>Member</dd>
      </div>
      <div>
        <dt>Status</dt>
        <dd>Active</dd>
      </div>
    </dl>
  </article>
</asp:Content>
