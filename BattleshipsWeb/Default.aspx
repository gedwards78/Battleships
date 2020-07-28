<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BattleshipsWeb._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>BATTLESHIPS</h1>
        <p class="lead">As the Admiral of the fleet it is up to you to defeat the enemy</p>
        <p><a target="_blank" href="https://en.wikipedia.org/wiki/Battleship_(game)" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Boards</h2>
            <p>
                Chose the size of the board from 8x8 up to 20x20. The bigger the board the greater the challenge
            </p>
            <p>
                <a class="btn btn-default" runat="server" href="~/config">Play &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Ships</h2>
            <p>
                With a little as 3 ships and as many as 10, finding one in the ocean can be a daunting task. 
                But if it is tricky for you then your opponent might not find you either. I would not count on it though
            </p>
            <p>
                <a class="btn btn-default" runat="server" href="~/config">Play &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Opponent</h2>
            <p>
                With clever game AI you will have to think fast to stay ahead of this opponent. 
                Once a target is found, your opponent will not stop until your ship is safely at the bottom of the sea
            </p>
            <p>
                <a class="btn btn-default" runat="server" href="~/config">Play &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
