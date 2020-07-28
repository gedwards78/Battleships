<%@ Page Title="Battle" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Battle.aspx.cs" Inherits="BattleshipsWeb.Battle" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <div id="heading" runat="server">
            <h1>The Battle Zone</h1>
        </div>

        <div id="gameArea">
            <div id="playerArea" runat="server">
                <h3>The Admiral</h3>
                <div id="playerBoardArea" runat="server"></div>
                <input id="bombLocation" name="bombLocation" type="hidden" value="3487" />
            </div>

            <div id="opponentArea" runat="server">
                <h3>Your Opponent</h3>
                <div id="opponentBoardArea" runat="server"></div>
            </div>

            <div id="strikeLogArea" runat="server">
                <h3>Strike Log</h3>
                <div id="strikeLog" runat="server"></div>
            </div>
        </div>
    </div>
    <div style="clear: both;"></div>
    <script>
        function cellClickTest(cell) {

            var location = $(cell).data("location");
            console.log('clicked: ' + location);
            //console.log('clicked: ' + cell.data("location"))

            if ($(cell).data("location") != undefined) {
                $(bombLocation).val(location);
                $("#formBattleships").submit();
                //window.location = "battle?location=" + location;
            }
        }

        $('#opponentBoard td').click(function () { cellClickTest(this); });
    </script>

</asp:Content>
