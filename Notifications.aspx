<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Notifications.aspx.cs" Inherits="NotificationsPage" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Notifications — Legacy Auth</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <article class="card">
    <h2>Inbox</h2>
    <p class="subtitle">Loaded from the Notifications table.</p>

    <ul class="notice-list">
      <asp:Literal ID="litNotices" runat="server" />
    </ul>
  </article>
</asp:Content>
