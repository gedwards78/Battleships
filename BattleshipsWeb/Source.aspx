<%@ Page Title="Source" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Source.aspx.cs" Inherits="BattleshipsWeb.Source" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Battleships game source</h3>
    <p>Solution consists of three projects</p>
    <ul>
        <li><b>Battleships.Web</b> - ASP.Net web application to demonstrate game logic</li>
        <li><b>BattleshipsCA</b> - Development application to test core game logic</li>
        <li><b>Battleships.Common</b> - Game logic</li>
    </ul>

    <p><a class="btn btn-default" target="_blank" href="https://github.com/gedwards78/Battleships">Access Solution &raquo;</a></p>
</asp:Content>
