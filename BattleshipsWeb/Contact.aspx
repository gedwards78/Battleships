<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="BattleshipsWeb.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Galit Edwards</h3>
<%--    <address>
        One Microsoft Way<br />
        Redmond, WA 98052-6399<br />
        <abbr title="Phone">P:</abbr>
        425.555.0100
    </address>--%>

    <address>
        <strong>Email:</strong> <a href="mailto:galit_edwards@yahoo.com">galit_edwards@yahoo.com</a><br />
        <strong>Phone:</strong> <span>07794428379</span>
    </address>
</asp:Content>
