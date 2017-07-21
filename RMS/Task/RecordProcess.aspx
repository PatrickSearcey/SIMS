<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RecordProcess.aspx.cs" Inherits="RMS.Task.RecordProcess" ValidateRequest="false" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/recordprocess.css" rel="stylesheet" />
    <script type="text/javascript">
        function OpenPopup(_URL) {
            open(_URL, 'Popup', 'toolbar=yes, menubar=no, width=840, height=500, scrollbars=yes');
        }
        function OnClientLoad(editor, args) {
            editor.setSize("1000px", "400px");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="lbUnlockPeriod">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlLocked" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAnalyze" />
                    <telerik:AjaxUpdatedControl ControlID="pnlApprove" />
                    <telerik:AjaxUpdatedControl ControlID="pnlReanalyze" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbFinishAnalyze">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlErrors" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAnalyze" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSaveAnalyze">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlErrors" />
                    <telerik:AjaxUpdatedControl ControlID="ltlSaved" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAnalyze" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbReanalyze">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlErrors" />
                    <telerik:AjaxUpdatedControl ControlID="pnlApprove" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbFinish">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlErrors" />
                    <telerik:AjaxUpdatedControl ControlID="pnlApprove" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlErrors" />
                    <telerik:AjaxUpdatedControl ControlID="ltlSaved" />
                    <telerik:AjaxUpdatedControl ControlID="pnlApprove" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="pnlAnalysisNotesEdit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAnalysisNotesEdit" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAnalysisNotesReadOnly" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNote" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="pnlAnalysisNotesReadOnly">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAnalysisNotesReadOnly" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAnalysisNotesEdit" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNote" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <asp:Panel ID="pnlDiagnostics" runat="server" CssClass="pnlLocked" Visible="false">
        <h4>Error Diagnostics</h4>
        <p>There was an error accessing the period. Please send the below details to the system administrator: <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W_Help_SIMS@usgs.gov</a></p>
        <p>Error: <asp:Literal ID="ltlErrorMsg" runat="server" /></p>
        <p>User ID: <asp:Literal ID="ltlUserID" runat="server" /></p>
        <p>Site ID: <asp:Literal ID="ltlSite" runat="server" /></p>
        <p>Record ID: <asp:Literal ID="ltlRecord" runat="server" /></p>
        <p>Period ID: <asp:Literal ID="ltlPeriod" runat="server" /></p>
        <p>Task: <asp:Literal ID="ltlTask" runat="server" /></p>
        <p>User WSC(s): <asp:Literal ID="ltlUserWSC" runat="server" /></p>
        <p>Site WSC: <asp:Literal ID="ltlSiteWSC" runat="server" /></p>
        <p>User Access: <asp:Literal ID="ltlAccess" runat="server" /></p>
    </asp:Panel>
    <asp:Panel ID="pnlErrors" runat="server" CssClass="pnlErrors" Visible="false">
        <h4>ACTION FAILED: Errors Found</h4>
        <p><asp:Literal ID="ltlError" runat="server" /></p>
    </asp:Panel>
    <asp:Panel ID="pnlLocked" runat="server" CssClass="pnlLocked">
        <h4>This record period is locked.</h4>
        <p>Lock Type: <asp:Literal ID="ltlLockType" runat="server" /></p>
        <p>Lock By: <asp:Literal ID="ltlLockBy" runat="server" /></p>
        <p>Lock Date: <asp:Literal ID="ltlLockDt" runat="server" /></p>
        <asp:Panel ID="pnlUnlock" runat="server" Visible="false">
            <hr />
            <asp:LinkButton ID="lbUnlockPeriod" runat="server" Text="Click here to unlock the period." OnCommand="lbUnlockPeriod_Command" CommandArgument="unlock" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel ID="pnlAnalyze" runat="server">
        <table class="RecordProcess">
            <tr>
                <td valign="top">
                    <h4>Setup the Period</h4>
                    <p>Hydrographer: <asp:Literal ID="ltlHydrographer" runat="server" /></p>
                    <p><span style="float:left;padding: 3px 5px 0 0;">Period Begin Date:</span> <telerik:RadDatePicker runat="server" ID="rdpBeginDateAnalyze" Skin="Bootstrap" /></p>
                    <p><span style="float:left;padding: 3px 18px 0 0;">Period End Date:</span> <telerik:RadDatePicker runat="server" ID="rdpEndDateAnalyze" Skin="Bootstrap" /></p>
                </td>
                <td valign="top">
                    <h4>Supporting Resources</h4>
                    <ul>
                        <li><asp:HyperLink ID="hlWYAnalysisNotes" runat="server" Text="View WY Analyses" /></li>
                        <li><asp:HyperLink ID="hlInstructions" runat="server" Text="WSC Analyzing Instructions" /></li>
                        <li><asp:HyperLink ID="hlNWISDataPortal" runat="server" Text="NWIS Data Portal and Reports" Target="_blank" NavigateUrl="https://reporting.nwis.usgs.gov/" /></li>
                        <li><asp:HyperLink ID="hlAutoReview" runat="server" Text="WSC Support Information (if applicable)" /></li>
                        <li><asp:HyperLink ID="hlWMAGuidelines" runat="server" Text="WMA Records Processing Guidelines" Target="_blank" NavigateUrl="https://drive.google.com/drive/folders/0BxDYBMoHxbICM2NWaWp5SXI0Ums" /></li>
                    </ul>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h4>Analysis from previous period</h4>
                    <asp:Panel ID="pnlAnalysisNotes" runat="server" Width="1000px" Height="300px" ScrollBars="Auto">
                        <asp:Literal ID="ltlPrevAnalysisNotes" runat="server" />
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h4>Analysis for this period</h4>
                    <p style="font-weight:bold;margin-top:-10px">Analyst Templates are automatically applied to new analyzing periods' analyses if a template has been assigned to the record-type.  Your WSC-level admin in your office can assign templates
                        to record-types via the Manage Record-Types interface from the Admin Tasks page.</p>
                    <asp:Panel id="pnlTemplateLink" runat="server" Visible="false">
                        <span style="font-weight:bold;">To view the detailed version of the template, which contains example text and guidance, please <asp:HyperLink ID="hlTemplate" runat="server" Target="_blank">CLICK HERE</asp:HyperLink>.</span>
                    </asp:Panel>  
                    <telerik:RadEditor ID="reAnalysisNotes" runat="server" Skin="Bootstrap" OnClientLoad="OnClientLoad" Width="100%" Height="300px" ExternalDialogsPath="~/EditorDialogs/">
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
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="text-align:center;">
                        <telerik:RadButton ID="rbFinishAnalyze" runat="server" Text="Finish Analyzing" OnCommand="Button_Commands" CommandArgument="Finish" CommandName="Analyze" UseSubmitBehavior="false" />
                        <telerik:RadButton ID="rbSaveAnalyze" runat="server" Text="Save" OnCommand="Button_Commands" CommandArgument="Save" CommandName="Analyze" UseSubmitBehavior="false" />
                        <telerik:RadButton ID="rbCancelAnalyze" runat="server" Text="Cancel" OnCommand="Button_Commands" CommandName="Cancel" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlApprove" runat="server">
        <table class="RecordProcess">
            <tr>
                <td valign="top">
                    <h4>General Period Details</h4>
                    <p>Analyzed By: <asp:Literal ID="ltlAnalyzedBy" runat="server" /></p>
                    <p>Approver: <asp:Literal ID="ltlApprover" runat="server" /></p>
                    <p>Time Period: <asp:Literal ID="ltlTimePeriod" runat="server" /></p>
                </td>
                <td valign="top">
                    <h4>Supporting Resources</h4>
                    <ul>
                        <li><asp:HyperLink ID="hlChangeLog" runat="server" Text="View Change Log" /></li>
                        <li><asp:HyperLink ID="hlDialog" runat="server" Text="View Dialog" /></li>
                        <li><asp:HyperLink ID="hlWYAnalysisNotes2" runat="server" Text="View WY Analyses" /></li>
                        <li><asp:HyperLink ID="hlApproveInst" runat="server" Text="WSC Approving Instructions" /></li>
                        <li><asp:HyperLink ID="hlAutoReview2" runat="server" Text="View Auto Review (if applicable)" /></li>
                    </ul>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h4>Analysis</h4>
                    <asp:Literal ID="ltlNote" runat="server" Text="<div style='width:100%;text-align:center;color:#ec562c;font-weight:bold;'>The analysis was saved!</div>" Visible="false" />
                    <asp:Panel ID="pnlAnalysisNotesReadOnly" runat="server" Width="1000px" Height="300px" ScrollBars="Auto">
                        <asp:Literal ID="ltlAnalysisNotes" runat="server" />
                        <div style="text-align:center;padding-top:5px;">
                            <telerik:RadButton ID="rbEditAnalysisNotes" runat="server" Text="Edit Analysis" OnCommand="EditAnalysisNotes" CommandArgument="Toggle" Visible="false" /> <!-- Control used to toggle display of pnlAnalysisNotesEdit, which is no longer in use -->
                        </div>
                    </asp:Panel>
                    <!-- APPROVERS ARE NOT ALLOWED TO EDIT STATION ANALYSIS NOTES -->
                    <!-- This section is not currently in use, per request of OWI; remains here in case user complaints trigger the re-release of the functionality -->
                    <asp:Panel ID="pnlAnalysisNotesEdit" runat="server">
                        <telerik:RadEditor ID="reAnalysisNotes2" runat="server" Skin="Bootstrap" OnClientLoad="OnClientLoad" Width="100%" Height="300px" ExternalDialogsPath="~/EditorDialogs/">
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
                        <div style="text-align:center;padding-top:5px;">
                            <telerik:RadButton ID="rbSaveAnalysisNotes" runat="server" Text="Save Changes" OnCommand="EditAnalysisNotes" CommandArgument="Save" UseSubmitBehavior="false" Visible="false" /> 
                            <telerik:RadButton ID="rbCancelAnalysisNotes" runat="server" Text="Cancel Without Saving" OnCommand="EditAnalysisNotes" CommandArgument="Cancel" Visible="false" />
                        </div>
                    </asp:Panel>
                    <!-- END LEGACY CONTROLS -->
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Panel ID="pnlApproverComments" runat="server">
                        <h4>Approver Comments</h4>
                        <div style="height:100px;width:1000px;overflow-y:scroll;">
                            <asp:Literal ID="ltlApproverComments" runat="server" />
                        </div>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h4>Comments</h4>
                    <telerik:RadEditor ID="reComments" runat="server" Skin="Bootstrap" OnClientLoad="OnClientLoad" Width="100%" Height="100px" ExternalDialogsPath="~/EditorDialogs/">
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
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="text-align:center;font-weight:bold;">
                        <p><asp:Literal ID="ltlReanalyzeNote" runat="server" Text="Ensure that all problems found with the record period have been documented in the comment box before sending back to analyst:<br />" />
                        <telerik:RadButton ID="rbReanalyze" runat="server" Text="Send Back for Reanalyzing" OnCommand="Button_Commands" CommandArgument="Reanalyze" CommandName="Approve" UseSubmitBehavior="false" />
                        <asp:Literal ID="ltlApproveNote" runat="server" Text="<br />By clicking approved you agree that you have followed current approval guidance and determined that the record period has been properly analyzed:<br />" />
                        <telerik:RadButton ID="rbFinish" runat="server" OnCommand="Button_Commands" CommandArgument="Finish" UseSubmitBehavior="false" />
                        </p>
                        <telerik:RadButton ID="rbSave" runat="server" Text="Save" OnCommand="Button_Commands" CommandArgument="Save" UseSubmitBehavior="false" />
                        <telerik:RadButton ID="rbCancel" runat="server" Text="Cancel" OnCommand="Button_Commands" CommandName="Cancel" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Literal ID="ltlSaved" Text="<div style='width:100%;text-align:center;color:#ec562c;font-weight:bold;'>The period data was saved!</div>" runat="server" Visible="false" />
</asp:Content>
