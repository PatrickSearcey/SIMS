<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RecordProcess.aspx.cs" Inherits="RMS.RecordProcess" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/recordprocess.css" rel="stylesheet" />
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
                        <li><asp:HyperLink ID="hlWYAnalysisNotes" runat="server" Text="View WY Analysis Notes" /></li>
                        <li><asp:HyperLink ID="hlInstructions" runat="server" Text="Analyzing Instructions" /></li>
                        <li><asp:HyperLink ID="hlAutoReview" runat="server" Text="View Auto Review (if applicable)" /></li>
                    </ul>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h4>Analysis notes from previous period</h4>
                    <telerik:RadTextBox ID="rtbPrevAnalysisNotes" runat="server" TextMode="MultiLine" Width="100%" Height="300px" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h4>Analysis notes for this period</h4>
                    <telerik:RadEditor ID="reAnalysisNotes" runat="server" Skin="Bootstrap" OnClientLoad="OnClientLoad" Width="100%" Height="300px">
                        <Tools>
                            <telerik:EditorToolGroup>
                                <telerik:EditorTool Name="AjaxSpellCheck" Visible="true" Enabled="true" />
                                <telerik:EditorTool Name="Bold" Visible="true" /> 
                            </telerik:EditorToolGroup>
                        </Tools>
                        <SpellCheckSettings DictionaryLanguage="en-GB" DictionaryPath="~/App_Data/RadSpell/" />
                    </telerik:RadEditor>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="text-align:center;">
                        <telerik:RadButton ID="rbFinishAnalyze" runat="server" Text="Finish Analyzing" OnCommand="Button_Commands" CommandArgument="Finish" CommandName="Analyze" />
                        <telerik:RadButton ID="rbSaveAnalyze" runat="server" Text="Save" OnCommand="Button_Commands" CommandArgument="Save" CommandName="Analyze" />
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
                        <li><asp:HyperLink ID="hlWYAnalysisNotes2" runat="server" Text="View WY Analysis Notes" /></li>
                        <li><asp:HyperLink ID="hlApproveInst" runat="server" Text="Approving Instructions" /></li>
                        <li><asp:HyperLink ID="hlAutoReview2" runat="server" Text="View Auto Review (if applicable)" /></li>
                    </ul>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h4>Analysis notes</h4>
                    <asp:Literal ID="ltlNote" runat="server" Text="<div style='width:100%;text-align:center;color:#ec562c;font-weight:bold;'>The analysis notes were saved!</div>" Visible="false" />
                    <asp:Panel ID="pnlAnalysisNotesReadOnly" runat="server">
                        <telerik:RadTextBox ID="rtbAnalysisNotes" runat="server" TextMode="MultiLine" Width="100%" Height="300px" ReadOnly="true" />
                        <div style="text-align:center;padding-top:5px;">
                            <telerik:RadButton ID="rbEditAnalysisNotes" runat="server" Text="Edit Analysis Notes" OnCommand="EditAnalysisNotes" CommandArgument="Toggle" />
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlAnalysisNotesEdit" runat="server">
                        <telerik:RadEditor ID="reAnalysisNotes2" runat="server" Skin="Bootstrap" OnClientLoad="OnClientLoad" Width="100%" Height="300px">
                            <Tools>
                                <telerik:EditorToolGroup>
                                    <telerik:EditorTool Name="AjaxSpellCheck" Visible="true" Enabled="true" />
                                    <telerik:EditorTool Name="Bold" Visible="true" /> 
                                </telerik:EditorToolGroup>
                            </Tools>
                            <SpellCheckSettings DictionaryLanguage="en-GB" DictionaryPath="~/App_Data/RadSpell/" />
                        </telerik:RadEditor>
                        <div style="text-align:center;padding-top:5px;">
                            <telerik:RadButton ID="rbSaveAnalysisNotes" runat="server" Text="Save Changes" OnCommand="EditAnalysisNotes" CommandArgument="Save" /> 
                            <telerik:RadButton ID="rbCancelAnalysisNotes" runat="server" Text="Cancel Without Saving" OnCommand="EditAnalysisNotes" CommandArgument="Cancel" />
                        </div>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Panel ID="pnlApproverComments" runat="server">
                        <h4>Approver Comments</h4>
                        <telerik:RadTextBox ID="rtbApproverComments" runat="server" TextMode="MultiLine" Width="100%" Height="100px" ReadOnly="true" />
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h4>Comments</h4>
                    <telerik:RadEditor ID="reComments" runat="server" Skin="Bootstrap" OnClientLoad="OnClientLoad" Width="100%" Height="100px">
                        <Tools>
                            <telerik:EditorToolGroup>
                                <telerik:EditorTool Name="AjaxSpellCheck" Visible="true" Enabled="true" />
                                <telerik:EditorTool Name="Bold" Visible="true" /> 
                            </telerik:EditorToolGroup>
                        </Tools>
                        <SpellCheckSettings DictionaryLanguage="en-GB" DictionaryPath="~/App_Data/RadSpell/" />
                    </telerik:RadEditor>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="text-align:center;">
                        <telerik:RadButton ID="rbReanalyze" runat="server" Text="Send Back for Reanalyzing" OnCommand="Button_Commands" CommandArgument="Reanalyze" CommandName="Approve" />
                        <telerik:RadButton ID="rbFinish" runat="server" OnCommand="Button_Commands" CommandArgument="Finish" />
                        <telerik:RadButton ID="rbSave" runat="server" Text="Save" OnCommand="Button_Commands" CommandArgument="Save" />
                        <telerik:RadButton ID="rbCancel" runat="server" Text="Cancel" OnCommand="Button_Commands" CommandName="Cancel" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Literal ID="ltlSaved" Text="<div style='width:100%;text-align:center;color:#ec562c;font-weight:bold;'>The period data was saved!</div>" runat="server" Visible="false" />
</asp:Content>
