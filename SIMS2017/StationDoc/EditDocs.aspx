<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="EditDocs.aspx.cs" Inherits="SIMS2017.StationDoc.EditDocs" ValidateRequest="false" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/stationdoc.css" rel="stylesheet" />
    <script type="text/javascript">
        function OpenSWR(_SWRurl) {
            open(_SWRurl, 'SWRPopup', 'toolbar=yes, menubar=no, width=840, height=500, scrollbars=yes');
        }
        function OnClientLoad(editor, args) {
            editor.setSize("1000px", "400px");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rrblAction">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseElement" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNote" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rddlElements">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNote" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseElement" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseAction" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmit1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNote" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseElement" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseAction" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbDelete">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNote" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseElement" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseAction" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbReset">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlEditAddElementInfo" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCancel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseElement" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseAction" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCancel1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseElement" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseAction" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCancel2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseElement" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseAction" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rlvElements">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlResult" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlChooseElement" />
                    <telerik:AjaxUpdatedControl ControlID="hfElementID" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNote" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
    <div class="linkbar">
        <div style="float:left;">
            <span style="font-weight:bold;">Note:&nbsp;&nbsp;SDESC, 
            <img src="../images/keycolor0.gif" alt="white color" /> = Station Description;
            &nbsp;&nbsp;SANAL, <img src="../images/keycolor1.gif" alt="gray color" /> = Station Analysis;
            &nbsp;&nbsp;MANU, <img src="../images/keycolor2.gif" alt="purple color" /> = Manuscripts</span>
        </div>
        <div style="float:right;">
            <asp:HyperLink ID="hlAutoReview" runat="server" target="_blank" Text="Click here to view Auto Review (if applicable)" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <asp:Panel ID="pnlNoAccess" runat="server">
            <h4>You do not have the necessary permission to modify station documents for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
        </asp:Panel>

        <asp:Panel ID="pnlNote" runat="server" CssClass="pnlNotes" Visible="false">
            <h4>ACTION SUCCESS!</h4>
            <p><asp:Literal ID="ltlNote" runat="server" /></p>
        </asp:Panel>

        <asp:Panel ID="pnlChooseAction" runat="server">
            <p class="EditDocList">1. Choose the action you wish to perform</p>
            <div class="EditDocList">
                <telerik:RadRadioButtonList ID="rrblAction" runat="server" OnSelectedIndexChanged="UpdateControls" AutoPostBack="true">
                    <Items>
                        <telerik:ButtonListItem Text="Edit Element" Value="Edit" />
                        <telerik:ButtonListItem Text="Add Element" Value="Add" />
                        <telerik:ButtonListItem Text="Delete Element" Value="Delete" />
                    </Items>
                </telerik:RadRadioButtonList>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlChooseElement" runat="server">
            <p class="EditDocList">2. Choose the element on which to perform the action (Do not copy and paste photos. Refer to photo guidance <a href="https://doimspp.sharepoint.com/:w:/s/SIMSDEV/Efxy3q5gCOBEvMBH5k8iFGIBSV_5uz6v1r4iHg1nLB5DCA" target="_blank">here</a>)</p>
            <div class="EditDocList">
                <telerik:RadDropDownList ID="rddlElements" runat="server" Skin="Bootstrap" Width="400px" DropDownHeight="400px" OnItemDataBound="rddlElements_ItemDataBound"
                    DataValueField="element_id" DataTextField="element_nm" OnSelectedIndexChanged="UpdateControls" AutoPostBack="true" />
                <telerik:RadListView ID="rlvElements" runat="server" Skin="Bootstrap" OnItemDataBound="rlvElements_ItemDataBound" DataKeyNames="ElementID">
                    <ItemTemplate>
                        <div class='<%# Eval("ReportType") %>'>
                            <div class="RevisionHistory">
                                Revised By: <%# Eval("RevisedBy") %> Date Revised: <%# String.Format("{0:MM/dd/yyyy}", Eval("RevisedDate")) %> 
                                (<a href='<%# String.Format("Archive.aspx?element_id={0}&site_id={1}", Eval("ElementID"), Eval("SiteID")) %>' target="_blank">revision history</a>)
                            </div>
                            <asp:Literal ID="ltlInactiveNotice" runat="server" />
                            <b><asp:LinkButton ID="lbElement" runat="server" Text='<%# Eval("ElementName") %>' OnCommand="lbElement_Command" CommandArgument='<%# Eval("ElementID") %>' />.--</b> 
                            <%# Eval("ElementInfo") %>
                            <p></p>
                        </div>
                    </ItemTemplate>
                </telerik:RadListView>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlResult" runat="server" CssClass="ResultPanel">
            <asp:HiddenField ID="hfElementID" runat="server" />
            <p class="EditDocList"><asp:Literal ID="ltlResultHeading" runat="server" /></p>
            <div class="RevisionHistory">
                <asp:Literal ID="ltlRevisedBy" runat="server" /><!-- Last Revised By: userid    Date Last Revised: date -->
                &nbsp;&nbsp;&nbsp;<asp:HyperLink ID="hlArchives" runat="server" Text="(revision history)" Target="_blank" /> <!-- Archive.aspx?element_id={0}&site_id={1} -->
            </div>
            <asp:Panel ID="pnlAutoGenerated" runat="server" CssClass="AutoGenerated">
                <h3>Auto-generated information:</h3>
                <asp:Literal ID="ltlAutoGenerated" runat="server" />
            </asp:Panel>
            <asp:Panel ID="pnlTelemetry" runat="server" Visible="false">
                <p>This element only consists of the auto-generated information.</p>
                <telerik:RadButton ID="rbSubmit" runat="server" Text="Submit" OnCommand="ButtonCommand" CommandArgument="Submit" UseSubmitBehavior="false" />
                <telerik:RadButton ID="rbCancel" runat="server" Text="Cancel" OnCommand="ButtonCommand" CommandArgument="Cancel" />
            </asp:Panel>
            <asp:Panel ID="pnlEditAddElementInfo" runat="server">
                <p>Revised By: <telerik:RadTextBox ID="rtbRevisedBy" runat="server" Width="140px" />  Revised Date: <telerik:RadTextBox ID="rtbRevisedDate" runat="server" Enabled="false" Width="140px" /></p>
                <asp:Panel ID="pnlDOLL" runat="server" Visible="false">
                    <table>
                        <tr>
                            <td>Enter last run date:</td>
                            <td>
                                <telerik:RadDatePicker runat="server" ID="rdpLastRunDt" Skin="Bootstrap" /> 
                                Edit frequency: <telerik:RadNumericTextBox ID="rntbFrequency" runat="server" Skin="Bootstrap" NumberFormat-DecimalDigits="0" Width="80px" /> year(s) &nbsp;&nbsp;
                                Close levels: <asp:CheckBox runat="server" ID="cbCloseLevels" />
                            </td>
                        </tr>
                    </table>
                    <asp:Literal ID="ltlDOLLInfo" runat="server" />
                    <asp:Literal ID="ltlSLAP" runat="server" /> <asp:HyperLink ID="hlSLAP" runat="server" Target="_blank" />
                </asp:Panel>
                <telerik:RadEditor ID="reElementInfo" runat="server" Skin="Bootstrap" OnClientLoad="OnClientLoad" Width="100%" Height="400px" ExternalDialogsPath="~/EditorDialogs/">
                    <Tools>
                        <telerik:EditorToolGroup>
                            <telerik:EditorTool Name="InsertLink" Text="Insert Link Dialog" />
                            <telerik:EditorTool Name="Bold" Text="Bold" Visible="true" /> 
                            <telerik:EditorTool Name="Indent" Text="Indent" Visible="true" />
                            <telerik:EditorTool Name="Outdent" Text="Outdent" Visible="true" />
                            <telerik:EditorTool Name="InsertImage" Text="Insert Image" Visible="true" />
                            <telerik:EditorTool Name="InsertTable" Text="Insert Table" Visible="true" />
                        </telerik:EditorToolGroup>
                    </Tools>
                </telerik:RadEditor>
                <br />
                <telerik:RadButton ID="rbSubmit1" runat="server" Text="Submit" OnCommand="ButtonCommand" CommandArgument="Submit" UseSubmitBehavior="false" />
                <telerik:RadButton ID="rbReset" runat="server" Text="Reset" OnCommand="ButtonCommand" CommandArgument="Reset" />
                <telerik:RadButton ID="rbCancel1" runat="server" Text="Cancel" OnCommand="ButtonCommand" CommandArgument="Cancel" />
                <span style="color:red;font-weight:bold"><asp:Literal ID="ltlError1" runat="server" /></span>
            </asp:Panel>
            <asp:Panel ID="pnlDeleteElement" runat="server">
                <p>Are you sure you want to delete this element?</p>
                <telerik:RadTextBox ID="rtbElementInfo" runat="server" TextMode="MultiLine" Width="1000px" Height="300px" ReadOnly="true" />
                <br /><br />
                <telerik:RadButton ID="rbDelete" runat="server" Text="Yes, delete!" OnCommand="ButtonCommand" CommandArgument="Delete" />
                <telerik:RadButton ID="rbCancel2" runat="server" Text="No, cancel" OnCommand="ButtonCommand" CommandArgument="Cancel" />
                <span style="color:red;font-weight:bold"><asp:Literal ID="ltlError2" runat="server" /></span>
            </asp:Panel>
        </asp:Panel>
    </div>
    <br /><br /><br />
</asp:Content>
