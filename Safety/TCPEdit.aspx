<%@ Page Title="" Language="C#" MasterPageFile="~/SafetySingleMenu.Master" AutoEventWireup="true" CodeBehind="TCPEdit.aspx.cs" Inherits="Safety.TCPEdit" ValidateRequest="false" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/reports.css" rel="stylesheet" />
    <style type="text/css">
        .imgPadding {
            padding-left:5px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rddlRemote">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlEditSiteSpecificInfo" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlEditSiteSpecificInfo" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlTCPs" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCancel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlEditSiteSpecificInfo" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dlTCPs">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dlTCPs" />
                    <telerik:AjaxUpdatedControl ControlID="ltlPlanVDoc" />
                    <telerik:AjaxUpdatedControl ControlID="pnlFileUpload" />
                    <telerik:AjaxUpdatedControl ControlID="imgUploadDocHelp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rddlFlow2Way">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rddlFlaggers" />
                    <telerik:AjaxUpdatedControl ControlID="rfvFlaggers" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rntbLaneNumber">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rddlFlaggers" />
                    <telerik:AjaxUpdatedControl ControlID="rfvFlaggers" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rntbShoulderWidth">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rddlFlaggers" />
                    <telerik:AjaxUpdatedControl ControlID="rfvFlaggers" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <telerik:RadPageLayout ID="rpl1" runat="server" GridType="Fluid" CssClass="mainContent">
        <Rows>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:CompositeLayoutColumn Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                        <Rows>
                            <telerik:LayoutRow>
                                <Content>
                                    <h4>Site Specific Information</h4>
                                    <asp:Panel ID="pnlEditSiteSpecificInfo" runat="server" CssClass="TCPInfo">
                                        <table style="width:600px">
                                            <tr>
                                                <td colspan="2" style="font-weight:bold;padding-bottom:10px;">
                                                    <asp:Literal ID="ltlLastUpdated" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="200">Remote Site?</td>
                                                <td>
                                                    <div class="floatLeft">
                                                        <telerik:RadDropDownList id="rddlRemote" runat="server" AutoPostBack="true" Width="100px" OnSelectedIndexChanged="rddlRemote_SelectedIndexChanged" Skin="Bootstrap">
                                                            <Items>
                                                                <telerik:DropDownListItem Text="" Value="" />
                                                                <telerik:DropDownListItem Text="Yes" Value="True" />
                                                                <telerik:DropDownListItem Text="No" Value="False" />
                                                            </Items>
                                                        </telerik:RadDropDownList>
                                                    </div>
                                                    <asp:Image ID="imgRemoteTT" runat="server" ImageURL="~/Images/tooltip.png" CssClass="imgPadding" />
                                                    <telerik:RadToolTip runat="server" ID="rttRemote" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                                        Height="90px" TargetControlID="imgRemoteTT" IsClientID="false" Animation="Fade" Position="TopRight">
                                                        Definition of remote: Greater than 30 ft from the highway; a sidewalk protected by a guard rail on the traffic side; a site that
                                                        is off-road 
                                                    </telerik:RadToolTip>
                                                    <asp:RequiredFieldValidator ID="rfvRemote" runat="server" ControlToValidate="rddlRemote" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Highway or Road Name</td>
                                                <td>
                                                    <telerik:RadTextBox ID="rtbRoadName" runat="server" Width="200px" Skin="Bootstrap" />
                                                    <asp:RequiredFieldValidator ID="rfvRoadName" runat="server" ControlToValidate="rtbRoadName" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Expressway?</td>
                                                <td>
                                                    <div class="floatLeft">
                                                        <telerik:RadDropDownList id="rddlExpressway" runat="server" Width="100px" Skin="Bootstrap">
                                                            <Items>
                                                                <telerik:DropDownListItem Text="" Value="" />
                                                                <telerik:DropDownListItem Text="Yes" Value="True" />
                                                                <telerik:DropDownListItem Text="No" Value="False" />
                                                            </Items>
                                                        </telerik:RadDropDownList>
                                                    </div>
                                                    <asp:Image ID="imgExpresswayTT" runat="server" ImageURL="~/Images/tooltip.png" CssClass="imgPadding" />
                                                    <telerik:RadToolTip runat="server" ID="rttExpressway" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                                        Height="60px" TargetControlID="imgExpresswayTT" IsClientID="false" Animation="Fade" Position="TopRight">
                                                        "A multi-lane highway designed for high-speed travel" (dictionary definition)
                                                    </telerik:RadToolTip>
                                                    <asp:RequiredFieldValidator ID="rfvExpressway" runat="server" ControlToValidate="rddlExpressway" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Bridge Width</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbBridgeWidth" runat="server"  Width="100px" NumberFormat-DecimalDigits="0" Skin="Bootstrap" /> feet
                                                    <asp:RequiredFieldValidator ID="rfvBridgeWidth" runat="server" ControlToValidate="rntbBridgeWidth" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Work Zone (Bridge Length)</td>
                                                <td>
                                                    <div class="floatLeft">
                                                        <telerik:RadNumericTextBox ID="rntbWorkZone" runat="server" Width="100px" NumberFormat-DecimalDigits="0" Skin="Bootstrap" /> feet
                                                    </div>
                                                    <asp:Image ID="imgWorkZoneTT" runat="server" ImageURL="~/Images/tooltip.png" CssClass="imgPadding" />
                                                    <telerik:RadToolTip runat="server" ID="rttWorkZone" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                                        Height="60px" TargetControlID="imgWorkZoneTT" IsClientID="false" Animation="Fade" Position="TopRight">
                                                        The distance between bridge abutments, "a structure that supports the ends of a bridge" (dictionary definition)
                                                    </telerik:RadToolTip>
                                                    <asp:RequiredFieldValidator ID="rfvWorkZone" runat="server" ControlToValidate="rntbWorkZone" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Lane Width</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbLaneWidth" runat="server" Width="100px" NumberFormat-DecimalDigits="0" Skin="Bootstrap" /> feet
                                                    <asp:RequiredFieldValidator ID="rfvLaneWidth" runat="server" ControlToValidate="rntbLaneWidth" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Shoulder Width</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbShoulderWidth" runat="server" Width="100px" OnTextChanged="RunPlanLogic" AutoPostBack="true" 
                                                        NumberFormat-DecimalDigits="0" Skin="Bootstrap" /> feet
                                                    <asp:RequiredFieldValidator ID="rfvShoulderWidth" runat="server" ControlToValidate="rntbShoulderWidth" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Speed Limit</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbSpeedLimit" runat="server" Width="100px" NumberFormat-DecimalDigits="0" Skin="Bootstrap" /> mph
                                                    <asp:RequiredFieldValidator ID="rfvSpeedLimit" runat="server" ControlToValidate="rntbSpeedLimit" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Number of Lanes</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbLaneNumber" runat="server" Width="100px" OnTextChanged="RunPlanLogic" AutoPostBack="true" 
                                                        NumberFormat-DecimalDigits="0" Skin="Bootstrap" />
                                                    <asp:RequiredFieldValidator ID="rfvLaneNumber" runat="server" ControlToValidate="rntbLaneNumber" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Traffic Flow</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlFlow2Way" runat="server" Width="100px" OnSelectedIndexChanged="RunPlanLogic" 
                                                        AutoPostBack="true" Skin="Bootstrap">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="two-way" Value="True" />
                                                            <telerik:DropDownListItem Text="one-way" Value="False" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvFlow2Way" runat="server" ControlToValidate="rddlFlow2Way" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Traffic Volume</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlTrafficVolume" runat="server" Width="100px" Skin="Bootstrap">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="low" Value="low" />
                                                            <telerik:DropDownListItem Text="moderate" Value="moderate" />
                                                            <telerik:DropDownListItem Text="high" Value="high" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvTrafficVolume" runat="server" ControlToValidate="rddlTrafficVolume" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Divided Highway?</td>
                                                <td>
                                                    <div class="floatLeft">
                                                        <telerik:RadDropDownList id="rddlDividedHighway" runat="server" Width="100px" Skin="Bootstrap">
                                                            <Items>
                                                                <telerik:DropDownListItem Text="" Value="" />
                                                                <telerik:DropDownListItem Text="Yes" Value="True" />
                                                                <telerik:DropDownListItem Text="No" Value="False" />
                                                            </Items>
                                                        </telerik:RadDropDownList>
                                                    </div>
                                                    <asp:Image ID="imgDividedHighwayTT" runat="server" ImageURL="~/Images/tooltip.png" CssClass="imgPadding" />
                                                    <telerik:RadToolTip runat="server" ID="rttDividedHighway" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                                        Height="60px" TargetControlID="imgDividedHighwayTT" IsClientID="false" Animation="Fade" Position="TopRight">
                                                        "Having the lanes for opposing traffic separated as a highway" (dictionary definition)
                                                    </telerik:RadToolTip>
                                                    <asp:RequiredFieldValidator ID="rfvDividedHighway" runat="server" ControlToValidate="rddlDividedHighway" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Median?</td>
                                                <td>
                                                    <div class="floatLeft">
                                                        <telerik:RadDropDownList id="rddlMedian" runat="server" Width="100px" Skin="Bootstrap">
                                                            <Items>
                                                                <telerik:DropDownListItem Text="" Value="" />
                                                                <telerik:DropDownListItem Text="Yes" Value="True" />
                                                                <telerik:DropDownListItem Text="No" Value="False" />
                                                            </Items>
                                                        </telerik:RadDropDownList>
                                                    </div>
                                                    <asp:Image ID="imgMedianTT" runat="server" ImageURL="~/Images/tooltip.png" CssClass="imgPadding" />
                                                    <telerik:RadToolTip runat="server" ID="rttMedian" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                                        Height="60px" TargetControlID="imgMedianTT" IsClientID="false" Animation="Fade" Position="TopRight">
                                                        "The dividing area, either landscaped or paved between opposing highway lanes" (dictionary definition)
                                                    </telerik:RadToolTip>
                                                    <asp:RequiredFieldValidator ID="rfvMedian" runat="server" ControlToValidate="rddlMedian" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Flaggers Required?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlFlaggers" runat="server" Width="100px" Skin="Bootstrap">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="True" />
                                                            <telerik:DropDownListItem Text="No" Value="False" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvFlaggers" runat="server" ControlToValidate="rddlFlaggers" ErrorMessage="* required" 
                                                        ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">Site Specific Notes<br />
                                                    <asp:Label ID="lblRemoteSiteNote" runat="server" Text="please explain why you chose Remote Site" Font-Bold="true" ForeColor="Red" Font-Italic="true" />
                                                </td>
                                                <td>
                                                    <telerik:RadTextBox ID="rtbNotes" runat="server" TextMode="MultiLine" Width="400px" Height="100px" Skin="Bootstrap" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top"><asp:Literal ID="ltlPlanVDoc" runat="server" Text="<b>Upload Plan V Document</b>" /></td>
                                                <td>
                                                    <asp:Panel ID="pnlFileUpload" runat="server" CssClass="floatLeft">
                                                        <telerik:RadAsyncUpload runat="server" ID="ruFile" AllowedFileExtensions="pdf" MaxFileInputsCount="1" 
                                                            MaxFileSize="524288000" DisableChunkUpload="true" MultipleFileSelection="Disabled" Skin="Bootstrap" PostbackTriggers="rbSubmit" 
                                                            Localization-Select="Browse" ToolTip="Documents must be in PDF format." />
                                                    </asp:Panel>
                                                    <asp:Image ID="imgUploadDocHelp" runat="server" ImageURL="~/Images/tooltip.png" CssClass="imgPadding" />
                                                    <telerik:RadToolTip runat="server" ID="rtt0" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                                        Height="100px" TargetControlID="imgUploadDocHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                                        If a plan document has already been uploaded, you will be able to see the link and download it under the TCP V below. Uploading
                                                        a new document will replace the current one. Only one plan document can be stored with TCP V.   
                                                    </telerik:RadToolTip>
                                                </td>
                                            </tr>
                                        </table>
                                        <hr />
                                        <asp:Literal ID="ltlNote" runat="server" /><br />
                                        <div style="padding-top:5px;">
                                            <telerik:RadButton ID="rbSubmit" runat="server" Text="Submit Changes" OnCommand="rbSubmit_Command" CommandArgument="SiteSpecificInfo" 
                                                AutoPostBack="true" Skin="Bootstrap" />
                                            <telerik:RadButton ID="rbCancel" runat="server" Text="Cancel and Reset" OnCommand="rbCancel_Command" CommandArgument="Cancel" 
                                                AutoPostBack="true" CausesValidation="false" Skin="Bootstrap" />
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </telerik:LayoutRow>
                        </Rows>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:CompositeLayoutColumn  Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                        <Rows>
                            <telerik:LayoutRow>
                                <Content>
                                    <h4>Traffic Control Plans</h4>
                                    <asp:Panel ID="pnlTCPs" runat="server" CssClass="TCPInfo">
                                        <asp:DataList ID="dlTCPs" runat="server" DataKeyField="TCPID" 
                                            OnEditCommand="dlTCPs_EditCommand"
                                            OnCancelCommand="dlTCPs_CancelCommand"
                                            OnUpdateCommand="dlTCPs_UpdateCommand" 
                                            OnItemCreated="dlTCPs_ItemCreated" 
                                            OnDeleteCommand="dlTCPs_DeleteCommand"
                                            OnItemDataBound="dlTCPs_ItemDataBound">
                                            <ItemTemplate>
                                                <a href='<%# Eval("TCPLink") %>'><%# Eval("TCPName") %></a><br />
                                                <div class="PlanInfo">
                                                    <b>Plan Specific Activity:</b><br />
                                                    <%# Eval("WorkAreaActivity") %><br />
                                                    <b>Plan Specific Notes:</b><br />
                                                    <%# Eval("PlanRemarks") %><br />
                                                    <asp:Panel ID="pnlDownloadPlan" runat="server">
                                                        <b>Download Plan Document:</b> <asp:Literal ID="ltlUploadPlan" runat="server" Text="<i>upload a plan in the site specific info above</i>" />
                                                            <asp:HyperLink ID="hlDownloadPlan" runat="server" Text="PlanVPlanDoc.pdf" Target="_blank" /><br />
                                                    </asp:Panel>
                                                    <asp:LinkButton ID="lbPlanRemarks" runat="server" Text="edit plan info" CommandName="edit" Font-Bold="true" /> |
                                                    <asp:LinkButton ID="lbDelete" runat="server" Text="delete plan" Font-Bold="true" CommandName="delete" />
                                                </div>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <a href='<%# Eval("TCPLink") %>'><%# Eval("TCPName") %></a><br />
                                                <div class="PlanInfo">
                                                    <b>Plan Specific Activity:</b>
                                                    <telerik:RadDropDownList ID="rddlWAA" runat="server" Skin="Bootstrap">
                                                        <Items>
                                                            <telerik:DropDownListItem Value="" />
                                                            <telerik:DropDownListItem Value="ADCP" Text="ADCP" />
                                                            <telerik:DropDownListItem Value="Bridge Board" Text="Bridge Board" />
                                                            <telerik:DropDownListItem Value="Wire Weight" Text="Wire Weight" />
                                                            <telerik:DropDownListItem Value="Power Equipment" Text="Power Equipment" />
                                                            <telerik:DropDownListItem Value="QW Monitor" Text="QW Monitor" />
                                                            <telerik:DropDownListItem Value="QW Sampling" Text="QW Sampling" />
                                                            <telerik:DropDownListItem Value="Construction" Text="Construction" />
                                                            <telerik:DropDownListItem Value="Other" Text="Other" />
                                                        </Items>
                                                    </telerik:RadDropDownList><br />
                                                    <b>Plan Specific Notes:</b><br />
                                                    <telerik:RadTextBox id="rtbPlanRemarks" runat="server" TextMode="MultiLine" Text='<%# Eval("PlanRemarks") %>' Font-Size="Small"
                                                        Width="400px" Skin="Bootstrap" /><br />
                                                    <asp:LinkButton ID="lbUpdateRemarks" runat="server" Text="save" CommandName="update" Font-Bold="true" /> |
                                                    <asp:LinkButton ID="lbCancel" runat="server" Text="cancel" CommandName="cancel" Font-Bold="true" />
                                                </div>
                                            </EditItemTemplate>
                                        </asp:DataList>
                                        <br />
                                        <asp:Image ID="imgBullet" runat="server" ImageUrl="~/images/bullet.png" AlternateText="bullet" /> <asp:LinkButton ID="lbAddPlanV" runat="server" Text="Click here to add TCP V - Plan Too Complicated" OnCommand="lbAddPlanV_Command" CommandArgument="AddV" />
                                    </asp:Panel>
                                </Content>
                            </telerik:LayoutRow>
                        </Rows>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
