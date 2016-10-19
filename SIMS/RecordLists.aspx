<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/RMS.Master" CodeBehind="RecordLists.aspx.vb" Inherits="SIMS.RecordLists" %>
<%@ MasterType  virtualPath="~/RMS.master" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript" language="javascript">
    function OnClientValueChanging(sender, args) {
        // Show the tooltip only while the slider handle is sliding. In case the user simply clicks on the track of the slider to change the value
        // the change will be quick and the tooltip will show and hide too quickly.
        if (!isSliding) return;

        var tooltip = $find("<%= RadToolTip1.ClientID %>");
        ResetToolTipLocation(tooltip);
        tooltip.set_text(args.get_newValue());
    }

    var isSliding = false;
    function OnClientSlideStart(sender, args) {
        isSliding = true;

        var tooltip = $find("<%= RadToolTip1.ClientID %>");
        ShowRadToolTip(tooltip, sender);
    }

    function OnClientSlideEnd(sender, args) {
        isSliding = false;

        var tooltip = $find("<%= RadToolTip1.ClientID %>");
        tooltip.hide();
    }

    function ShowRadToolTip(tooltip, slider) {
        var activeHandle = slider.get_activeHandle();
        if (!activeHandle) return;

        tooltip.set_targetControl(activeHandle);
        ResetToolTipLocation(tooltip);
    }

    function ResetToolTipLocation(tooltip) {
        if (!tooltip.isVisible())
            tooltip.show();
        else
            tooltip.updateLocation();
    }
</script>
<style type="text/css">
        .formRow
        {
            float: left;
            border-right: solid 1px #cbcbcb;
            height: 220px;
            padding-left: 16px;
        }
        /* ie 7 does not use same size as other browsers */
        h5
        {
            font-size: 10px;
        }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:Panel ID="pnlHasAccess" runat="server">
    <telerik:RadAjaxPanel ID="rapMain" runat="server">
        <telerik:RadToolTip ID="RadToolTip1" runat="server" OffsetY="3" Position="TopCenter"
            ShowCallout="false" Height="20px" ShowEvent="fromcode" HideEvent="FromCode">
        </telerik:RadToolTip>
        <table>
            <tr>
                <td>
                    <fieldset style="height:100px;width:350px;">
                        <legend>Explanation</legend>
                        <div style="padding:5px;">
                        <label>Click on the record-type description to <asp:Label ID="lblListType" runat="server" /> the record.</label><br />
                        <label>Click on the site number to open the Station Information page.</label><br />
                        <img border="0" src="images/lock.png" alt="lock" style="padding-right:10px;padding-left:5px;" />
                        <label>Site currently locked</label><br />
                        <img border="0" src="images/save_icon.gif" alt="updates pending" 
                            style="padding-right:12px;padding-left:5px;" />
                        <label>Updates pending</label>
                        </div>
                    </fieldset>
                </td>
                <td>
                    <fieldset style="height:100px;width:250px;">
                        <legend>Quick Links</legend>
                        <div style="padding:5px;">
                            <asp:Literal ID="ltlQuickLinks" runat="server" />
                        </div>
                    </fieldset>
                    <fieldset style="height:95px;width:400px;display:none;">
                        <legend>Advanced Filter</legend>
                        <div class="formRow" style="border: 0;">
                            <h5>view records within a range of days since last approved</h5>
                            <table>
                                <tr>
                                    <td>
                                        0</td>
                                    <td>
                                        <telerik:RadSlider runat="server" ID="rsDaysAgo" IsSelectionRangeEnabled="true"
                                            MinimumValue="0" MaximumValue="500" SmallChange="1" SelectionStart="0" SelectionEnd="500"
                                            OnClientValueChanging="OnClientValueChanging" OnValueChanged="rsDaysAgo_ValueChanged"
                                            OnClientSlideStart="OnClientSlideStart" OnClientSlideEnd="OnClientSlideEnd" AutoPostBack="true"
                                            ShowDecreaseHandle="false" ShowIncreaseHandle="false" Skin="Sunset" />
                                    </td>
                                    <td>
                                        500</td>
                                </tr>
                            </table>
                            <label>selection range:</label> <asp:Label ID="lblSelectionStart" runat="server" Text="0"></asp:Label> -
                            <asp:Label ID="lblSelectionEnd" runat="server" Text="500"></asp:Label> <label>days</label>
                        </div>
                    </fieldset>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hfRangeStart" runat="server" />
        <asp:HiddenField ID="hfRangeEnd" runat="server" />
        <asp:HiddenField ID="hfListType" runat="server" />
        <asp:PlaceHolder ID="phRecordsToProcess" runat="server"></asp:PlaceHolder>
    </telerik:RadAjaxPanel>
</asp:Panel>
</asp:Content>