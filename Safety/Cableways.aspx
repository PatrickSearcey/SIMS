<%@ Page Title="" Language="C#" MasterPageFile="~/SafetySingleMenu.Master" AutoEventWireup="true" CodeBehind="Cableways.aspx.cs" Inherits="Safety.Cableways" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/reports.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgCableways">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgCableways" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lblError" />
                    <telerik:AjaxUpdatedControl ControlID="lblSuccess" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbToggleRStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbToggleRStatus" />
                    <telerik:AjaxUpdatedControl ControlID="rgCableways" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap"></telerik:RadAjaxLoadingPanel>

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
    <asp:Panel ID="pnlHasAccess" runat="server">
        <div style="height:50px;width:100%;padding-bottom:10px;">
            <p style="font-size:10pt;">A cableway is defined as any <b>permanent bank-supported</b> aerial conveying system suspended above a waterway for the 
            purpose of making hydrologic measurements. Cableways typically are classified as either: (1) <b>manned cableways</b> where the 
            hydrographer traverses the river in a cable car suspended from the main cable to operate measurement equipment; or (2) 
            <b>bank-operated cableways</b> (BOS) where the hydrographer is positioned on the stream bank to remotely operate cableway-
            suspended measurement equipment.</p>
        </div>
        <div style="height:60px;width:100%;">
            <div style="float:left;">
                <asp:Label ID="lblError" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblSuccess" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Green"></asp:Label><br />
                <asp:LinkButton ID="lbToggleRStatus" runat="server" OnCommand="lbToggleRStatus_Command" Font-Bold="true" />
            </div>
            <div style="float:right;">
                <asp:ImageMap ID="imInstructions" runat="server" ImageUrl="~/images/manualandhelplinks.png" BorderStyle="None">
                    <asp:RectangleHotSpot AlternateText="Manned Cableway Inspection e-Form" HotSpotMode="Navigate" 
                        NavigateUrl="http://sims.water.usgs.gov/SIMSShare/MannedCablewayInspectionE-form.pdf" Bottom="33" Left="38" Right="210" Top="15" Target="_blank" />
                    <asp:RectangleHotSpot AlternateText="Bank-Operated Cableways Inspection Checklist" HotSpotMode="Navigate" 
                        NavigateUrl="http://sims.water.usgs.gov/SIMSShare/Bank-OperatedCablewayInspectionE-form.pdf" Bottom="58" Left="38" Right="210" Top="38" Target="_blank" />
                    <asp:RectangleHotSpot AlternateText="USGS Manual" HotSpotMode="Navigate" 
                        NavigateUrl="https://www2.usgs.gov/usgs-manual/handbook/hb/445-2-h/ch41.html" Bottom="33" Left="270" 
                        Right="470" Target="_blank" Top="10" />
                    <asp:RectangleHotSpot AlternateText="WMA Memo 13.03" HotSpotMode="Navigate"
                        NavigateUrl="http://sims.water.usgs.gov/SIMSShare/WMAMemorandum13.03.pdf" Bottom="60" Left="270" Right="460" Target="_blank" Top="38" />
                    <asp:RectangleHotSpot AlternateText="CMI Instructions (.pptx)" HotSpotMode="Navigate" 
                        NavigateUrl="http://sims.water.usgs.gov/SIMSShare/SIMSCMIInstructions.pptx" Left="520" 
                        Right="800" Target="_blank" Top="5" Bottom="60" />
                </asp:ImageMap>
            </div>
        </div>
        <telerik:RadGrid ID="rgCableways" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" RenderMode="Lightweight" 
            Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="50"
            AllowSorting="true" 
            AllowMultiRowSelection="false" 
            AllowFiltering="true"
            AllowPaging="true"
            AllowAutomaticDeletes="true" 
            OnNeedDataSource="rgCableways_NeedDataSource" 
            OnDetailTableDataBind="rgCableways_DetailTableDataBind"
            OnItemDataBound="rgCableways_ItemDataBound"
            OnInsertCommand="rgCableways_InsertCommand"
            OnUpdateCommand="rgCableways_UpdateCommand"
            OnDeleteCommand="rgCableways_DeleteCommand"
            OnPreRender="rgCableways_PreRender">
            <PagerStyle Mode="NumericPages" />
            <MasterTableView DataKeyNames="cableway_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="Top" 
                Name="Cableways" AllowFilteringByColumn="true">
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="cableway_visit_id" Width="100%" runat="server" CommandItemDisplay="Top"
                        Name="Visits">
                        <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="cableway_id" MasterKeyField="cableway_id" />
                        </ParentTableRelation>  
                        <CommandItemSettings AddNewRecordText="Add New Visit" ShowRefreshButton="false" />
                        <Columns>
                            <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn2">
                                <HeaderStyle Width="20px" />
                                <ItemStyle CssClass="MyImageButton" />
                            </telerik:GridEditCommandColumn>
                            <telerik:GridBoundColumn DataField="visit_dt" HeaderText="Visit Date" DataFormatString="{0:MM/dd/yyyy}" 
                                UniqueName="visit_dt" SortExpression="visit_dt" AllowFiltering="false" HeaderStyle-Width="100" />
                            <telerik:GridBoundColumn DataField="type_cd_desc" HeaderText="Visit Type" UniqueName="type_cd_desc" HeaderStyle-Width="180" 
                                SortExpression="visit_type_cd" AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="action_cd_desc" HeaderText="Visit Action" UniqueName="action_cd_desc" HeaderStyle-Width="220"
                                SortExpression="visit_action_cd" AllowFiltering="false" />
                            <telerik:GridTemplateColumn HeaderText="Documentation" HeaderStyle-Width="300" DataField="visit_file_nm">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlDoc" runat="server" Text='<%# Bind("visit_file_nm") %>' Target="_blank" />&nbsp;
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="remarks" HeaderText="Remarks" SortExpression="remarks" UniqueName="remarks" AllowFiltering="false" />
                            <telerik:GridButtonColumn ConfirmText="Delete this visit?" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                UniqueName="VisitDeleteColumn">
                                <HeaderStyle Width="20px" />
                                <ItemStyle HorizontalAlign="Center" CssClass="MyImageButton" />
                            </telerik:GridButtonColumn>
                        </Columns>

                        <EditFormSettings EditFormType="Template">
                            <FormTemplate>
                                <div style="padding:5px;background-color:#f4cebf;">
                                    <table id="tableForm1" cellspacing="5" cellpadding="5" width="100%" border="0" rules="none" style="border-collapse: collapse; background: #f4cebf;">
                                        <tr>
                                            <td colspan="2">
                                                <h4><asp:Literal ID="ltlDetailsEditFormTitle" runat="server" /></h4>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width:120px;">
                                                <label>Visit Date:</label>
                                            </td>
                                            <td>
                                                <telerik:RadDatePicker ID="rdpVisit" runat="server" MinDate="1/1/1900" />
                                                <asp:Image ID="imgVisitDateHelp" runat="server" ImageURL="~/Images/tooltip.png" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Visit Type:</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlVisitType" runat="server" DataTextField="type_cd_desc"
                                                    DataValueField="visit_type_cd" Width="400px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Cableway Assessment:</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlVisitAction" runat="server" DataTextField="action_cd_desc"
                                                    DataValueField="visit_action_cd" Width="400px" />
                                                <asp:Image ID="imgVisitActionHelp" runat="server" ImageURL="~/Images/tooltip.png" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Remarks:</label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="tbRemarks" runat="server" Width="400px" Height="100px" TextMode="MultiLine" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label><asp:Label ID="lblUploadDoc" runat="server" Text="Upload Document: " /></label>
                                            </td>
                                            <td>
                                                <div style="float:left;">
                                                    <telerik:RadAsyncUpload runat="server" ID="rauUpload" TemporaryFolder="~/Control/Temporary/" AllowedFileExtensions="pdf,docx,txt" MaxFileInputsCount="1" 
                                                        MaxFileSize="524288000" DisableChunkUpload="true" MultipleFileSelection="Disabled" Skin="Bootstrap" PostbackTriggers="btnInsert1,btnUpdate1" 
                                                        Localization-Select="Browse" ToolTip="Documents can be one of the following formats: .pdf, .docx, .txt" />
                                                </div>
                                                <asp:Image ID="imgUploadDocHelp" runat="server" ImageURL="~/Images/tooltip.png" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <asp:Button ID="btnInsert1" Text="Insert" runat="server" CommandName="PerformInsert" />
                                                <asp:Button ID="btnUpdate1" Text="Update" runat="server" CommandName="Update" />
                                                <asp:Button ID="btnCancel1" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <telerik:RadToolTip runat="server" ID="rtt99" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                    Height="50px" TargetControlID="imgVisitDateHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    Enter the date of the visit. If you are adding non-visit information, enter today's date.
                                </telerik:RadToolTip>
                                <telerik:RadToolTip runat="server" ID="rtt0" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                    Height="90px" TargetControlID="imgUploadDocHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    One supporting document may be uploaded. Please upload any hand written inspection forms. To include images or other supporting documents, please merge them into one file.   
                                </telerik:RadToolTip>
                                <telerik:RadToolTip runat="server" ID="rtt1" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                    Height="80px" TargetControlID="imgVisitActionHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    What was the outcome of this visit? What you choose here might impact your Cableway Status, and you may need to edit the cableway above as well.
                                </telerik:RadToolTip>
                            </FormTemplate>
                        </EditFormSettings>
                        <NoRecordsTemplate>
                            <div style="background-color:#d7adad;padding:5px;font-weight:bold;">No Site Visits have been recorded.</div>
                        </NoRecordsTemplate>
                        <EditItemStyle BackColor="#f4cebf" />
                        <ItemStyle BackColor="#f4eebf" />
                        <AlternatingItemStyle BackColor="#f4eebf" />
                    </telerik:GridTableView>
                </DetailTables>
                <CommandItemSettings AddNewRecordText="Add New Cableway" ShowRefreshButton="false" />
                <Columns>
                    <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn1">
                        <HeaderStyle Width="20px" />
                        <ItemStyle CssClass="MyImageButton" />
                    </telerik:GridEditCommandColumn>
                    <telerik:GridBoundColumn DataField="site_no_nm" HeaderText="Site" UniqueName="site_no_nm" SortExpression="site_no_nm" FilterControlWidth="150px" />
                    <telerik:GridBoundColumn DataField="status_cd_desc" HeaderText="Cableway Status" UniqueName="status_cd_desc" SortExpression="cableway_status_cd" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="type_cd_desc" HeaderText="Cableway Type"  UniqueName="type_cd_desc" SortExpression="cableway_type_cd" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="cableway_inspection_freq" HeaderText="Insp Freq" UniqueName="cableway_inspection_freq" SortExpression="cableway_inspection_freq" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="aerial_marker_req" HeaderText="Aerial Marker Required" UniqueName="aerial_marker_req" SortExpression="aerial_marker_req" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="aerial_marker_inst" HeaderText="Aerial Marker Installed" UniqueName="aerial_marker_inst" SortExpression="aerial_marker_inst" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="office_cd" HeaderText="Office" UniqueName="office_cd" ReadOnly="true" FilterControlWidth="30px" />
                </Columns>

                <EditFormSettings EditFormType="Template">
                    <FormTemplate>
                        <div style="padding:5px;background-color:#d1ede5;">
                            <table id="tableForm1" cellspacing="5" cellpadding="5" width="800" border="0" rules="none" style="border-collapse: collapse; background: #d1ede5;">
                                <tr>
                                    <td colspan="2">
                                        <h4><asp:Literal ID="ltlEditFormTitle" runat="server" /></h4>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:200px;">
                                        <label>Site:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSites" runat="server" Width="500px" DataTextField="site_no_nm" DataValueField="site_id" />
                                        <asp:Image ID="imgSitesHelp" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Nickname:</label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbNickname" runat="server" Width="200px" MaxLength="30" />
                                        <asp:Image ID="imgNicknameHelp" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Cableway Status:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCablewayStatus" runat="server" Width="400px" DataTextField="status_cd_desc" DataValueField="cableway_status_cd" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Cableway Type:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCablewayType" runat="server" Width="400px" DataTextField="type_cd_desc" DataValueField="cableway_type_cd" />
                                        <asp:Image ID="imgCablewayTypeHelp" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap>
                                        <label>Inspection Frequency:</label>
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="rntbFreq" runat="server" Width="50px" NumberFormat-DecimalDigits="0" MaxLength="3" />
                                        <asp:Image ID="imgFreqHelp" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap>
                                        <label>Are Aerial Markers:</label><br />
                                        <span style="padding-left:90px;"><label>Required?</label></span><br />
                                        <span style="padding-left:90px;"><label>Installed?</label></span>
                                    </td>
                                    <td>
                                        <br />
                                        <asp:RadioButtonList ID="rblAerialMarkerReq" runat="server" RepeatDirection="Horizontal" Style="float:left;">
                                            <asp:ListItem Text="Yes" Value="Y" />
                                            <asp:ListItem Text="No" Value="N" />
                                            <asp:ListItem Text="Unknown" Value="U" />
                                        </asp:RadioButtonList>
                                        &nbsp;&nbsp;<asp:Image ID="imgAerialMarkerReq" runat="server" ImageUrl="~/Images/tooltip.png" /><br /><br />
                                        <asp:RadioButtonList ID="rblAerialMarkerInst" runat="server" RepeatDirection="Horizontal" Style="float:left;">
                                            <asp:ListItem Text="Yes" Value="Y" />
                                            <asp:ListItem Text="No" Value="N" />
                                            <asp:ListItem Text="Unknown" Value="U" />
                                        </asp:RadioButtonList>
                                        &nbsp;&nbsp;<asp:Image ID="imgAerialMarkerInst" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="2">
                                        <asp:Button ID="btnInsert2" Text="Insert" runat="server" CommandName="PerformInsert" />
                                        <asp:Button ID="btnUpdate2" Text="Update" runat="server" CommandName="Update" />
                                        <asp:Button ID="btnCancel2" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <telerik:RadToolTip runat="server" ID="rtt3" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="60px" TargetControlID="imgSitesHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                            Only sites that already exist in SIMS are listed.  If you do not see your site, you must first add it to SIMS.  
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rrt5" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="60px" TargetControlID="imgNicknameHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                            Enter a short descriptive phrase or word for this cableway. This is useful for setting your cableways apart if you have multiples at a site.
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rtt6" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="100px" TargetControlID="imgCablewayTypeHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                            <b>Bank Operated Cableway</b> A manually operated "loop" cableway<br />
                            <b>Manned Cableway</b> Traditional cableway with cable car and operator<br />
                            <b>Type Unknown</b> You just don't know
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rtt7" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="50px" TargetControlID="imgFreqHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                            Enter in years the frequency of inspection visits. This is a requirement to operate the cableway.
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rtt8" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            height="50px" TargetControlID="imgAerialMarkerReq" IsClientID="false" Animation="Fade" Position="TopRight">
                            Select yes if an aerial marker is required for this cableway, or no if it is not.
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rtt9" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            height="50px" TargetControlID="imgAerialMarkerInst" IsClientID="false" Animation="Fade" Position="TopRight">
                            Select yes if an aerial marker exists for this cableway, or no if it does not.
                        </telerik:RadToolTip>
                    </FormTemplate>
                </EditFormSettings>
                <EditItemStyle BackColor="#d1ede5" />
            </MasterTableView>
        </telerik:RadGrid>
    </asp:Panel>
    </div>
</asp:Content>
