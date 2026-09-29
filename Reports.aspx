<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Reports.aspx.cs" Inherits="ReportsPage" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">Reports — Legacy Auth</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <article class="card">
    <h2>Weekly reports</h2>
    <p class="subtitle">Loaded from the Reports table.</p>

    <div class="table-wrap">
      <table class="data-table">
        <thead>
          <tr>
            <th>Report</th>
            <th>Owner</th>
            <th>Period</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          <asp:Literal ID="litRows" runat="server" />
        </tbody>
      </table>
    </div>
  </article>
</asp:Content>
