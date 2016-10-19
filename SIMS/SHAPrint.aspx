<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SHAPrint.aspx.vb" Inherits="SIMS.SHAPrint" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Site Hazard Analysis</title>
    <link rel="stylesheet" type="text/css" href="css/styles.css" />
    <link rel="stylesheet" type="text/css" href="css/common.css" />
    <link rel="stylesheet" type="text/css" href="css/custom.css" />
    <telerik:RadStyleSheetManager ID="rssm" runat="server">
        <StyleSheets>
            <Telerik:StyleSheetReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Skins.Editor.css" />
            <Telerik:StyleSheetReference Assembly="Telerik.Web.UI.Skins" Name="Telerik.Web.UI.Skins.Web20.Editor.Web20.css" />
            <Telerik:StyleSheetReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Skins.Window.css" />
            <Telerik:StyleSheetReference Assembly="Telerik.Web.UI.Skins" Name="Telerik.Web.UI.Skins.Web20.Window.Web20.css" />
        </StyleSheets>
    </telerik:RadStyleSheetManager>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width:670px;padding:20px;">
    <telerik:RadScriptManager ID="rsm" runat="server" />
    <telerik:RadFormDecorator ID="rfd" runat="server" Skin="Web20" DecoratedControls="All" />
    <asp:Panel ID="pnlNoSite" runat="server">
        <h2>An error has occurred. No site submitted.</h2>
    </asp:Panel>
    <asp:Panel ID="pnlSHA" runat="server">

        <!-- PAGE HEADER -->
        <div style="height:120px;">
            <div style="float:left;font-weight:bold;">
                <span style="font-size:12pt;">Site Hazard Analysis</span><br /><br />
                <asp:Literal ID="ltlSiteNoName" runat="server" /><br />
                Report generated on <asp:Literal ID="ltlDate" runat="server" />
            </div>
            <div style="float:right;">
                <b>Responsible Office:</b><br />
                U.S. Geological Survey<br />
                <asp:Literal ID="ltlOfficeName" runat="server" /><br />
                <asp:Literal ID="ltlOfficeAddress" runat="server" /><br />
                <asp:Literal ID="ltlOfficeCity" runat="server" /><br />
                <asp:Literal ID="ltlOfficeNumber" runat="server" />
            </div>
        </div>

        <!-- EMERGENCY INFORMATION -->
        <div style="overflow:hidden;">
            <h3 class="sectionHeadings">Emergency Information</h3>
            <b><asp:Literal ID="ltlEmergService" runat="server" /><br />
            <asp:Literal ID="ltlCellService" runat="server" /></b>
            <div style="width:100%;">
                <div style="float:left;width:49%;padding-bottom:10px;">
                    <h4 class="sectionHeadings">Emergency Contacts</h4><br />
                    <asp:ListView runat="server" ID="lvEmergContacts">
                        <LayoutTemplate>
                            <div runat="server" id="itemPlaceholder"></div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div id="Div2" runat="server" style="border:1px solid #cccccc;padding:5px;">
                                <p style="margin-top:0px;"><%# Eval("contact_nm")%><br />
                                <%# Eval("street_addrs")%><br />
                                <%# Eval("city")%>, <%# Eval("state")%> <%# Eval("zip")%><br />
                                <%# Eval("ph_no")%></p>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p class="noData">No contacts exist for this site.</p>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
                <div style="float:right;width:49%;padding-bottom:10px;">
                    <h4 class="sectionHeadings">Nearest Hospitals</h4><br />
                    <asp:ListView runat="server" ID="lvHospitals">
                        <LayoutTemplate>
                            <div runat="server" id="itemPlaceholder"></div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div id="Div3" runat="server" style="border:1px solid #cccccc;padding:5px;">
                                <p style="margin-top:0px;"><%# Eval("hospital_nm")%><br />
                                <%# Eval("street_addrs")%><br />
                                <%# Eval("city")%>, <%# Eval("state")%> <%# Eval("zip")%><br />
                                <%# Eval("dec_lat_va")%>, <%# Eval("dec_long_va")%><br />
                                <%# Eval("ph_no")%></p>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p class="noData">No hospitals exist for this site.</p>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </div>
        </div>

        
        <!-- SERVICING SITE HAZARD ANALYSIS -->
        <div>
            <h3 class="sectionHeadings">Safely Servicing This Site</h3>
            <p>When servicing this site, be aware of the following general hazards:</p>
            <asp:ListView runat="server" ID="lvServicingSiteSpecificCond" OnItemDataBound="lvServicingSiteSpecificCond_ItemDataBound">
                <LayoutTemplate>
                    <ul>
                        <li runat="server" id="itemPlaceholder"></li>
                    </ul>
                </LayoutTemplate>
                <ItemTemplate>
                    <li><%# Eval("servicing_va")%></li>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <li class="noData">No servicing site specific hazards have been added.</li>
                </EmptyDataTemplate>
            </asp:ListView>
            <p>Recommended equipment regardless of activity:</p>
            <asp:ListView runat="server" ID="lvServicingSiteRecEquip">
                <LayoutTemplate>
                    <ul>
                        <li runat="server" id="itemPlaceholder"></li>
                    </ul>
                </LayoutTemplate>
                <ItemTemplate>
                    <li><%# Eval("recom_equip")%></li>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <li class="noData">No equipment has been added.</li>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        
        <!-- JOB HAZARD ANALYSES -->
        <div>
            <h3 class="sectionHeadings" style="padding-top:10px;">Making Safe Measurements</h3>
            <p>Potential safety hazards associated with making measurements.</p>
            <asp:Panel ID="pnlDischargeMeas" runat="server" Visible="false">
                <h4 class="sectionHeadings">DISCHARGE MEASUREMENTS</h4>
                <div style="padding-left:10px;">
                    <p style="font-size:8pt;margin-top:5px;">Revised by: <asp:Literal ID="ltlDMRevisedBy" runat="server" /> &nbsp;&nbsp;&nbsp; Date revised: <asp:Literal ID="ltlDMRevisedDate" runat="server" /></p>
                    <asp:Literal ID="ltlDischargeMeas" runat="server" />
                    <br /><br />
                    <h5 class="sectionHeadings">Measurement Specific Job Hazards</h5><br />
                    <asp:ListView ID="lvDischargeMeasJHA" runat="server" DataKeyNames="site_jha_id">
                        <LayoutTemplate>
                            <div id="itemPlaceholder" runat="server" />
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div id="Div1" runat="server" style="border:1px solid #999999;padding:5px 5px 5px 5px;">
                                <p style="font-weight:bold;margin-top:0px;"><%# Eval("jha_description")%></p>
                                <asp:ListView ID="lvDischargeMeasRemarks" runat="server" DataSource='<%# getRemarksForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_specificcond_id">
                                    <LayoutTemplate>
                                        <ul>
                                            <li runat="server" id="itemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li><%# Eval("remarks")%></li>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <span class="noData">No remarks have been entered.</span>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                                <div style="padding-left:30px;">
                                    <p style="font-weight:bold;margin-bottom:0px;">Know Your Limits</p>
                                    <asp:ListView ID="lvDischargeMeasOpLimits" runat="server" DataSource='<%# getJobOpLimitsForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_reflevel_id">
                                        <LayoutTemplate>
                                            <table cellpadding="3" style="border: 1px solid #cccccc;width:100%;">
                                                <tr>
                                                    <th style="font-size:smaller;width:150px;">Limit Type</th>
                                                    <th style="font-size:smaller;width:100px;">Limit Value</th>
                                                    <th style="font-size:smaller;">Remarks</th>
                                                </tr>
                                                <tr runat="server" id="itemPlaceholder" />
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr id="Tr1" runat="server">
                                                <td><%# Eval("reflevel_tp")%>&nbsp;</td>
                                                <td align="center"><%# Eval("reflevel_va")%>&nbsp;<%# Eval("reflevel_units")%></td>
                                                <td><%# Eval("remarks")%>&nbsp;</td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <span class="noData">No job operational limits have been entered.</span>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p>No activity specific job hazards have been entered.</p>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </asp:Panel>
            <br /><br />
            <asp:Panel ID="pnlGWMeas" runat="server" Visible="false">
                <h4 class="sectionHeadings">GROUNDWATER MEASUREMENTS</h4>
                <div style="padding-left:10px;">
                    <p style="font-size:8pt;margin-top:5px;">Revised by: <asp:Literal ID="ltlGWRevisedBy" runat="server" /> &nbsp;&nbsp;&nbsp; Date revised: <asp:Literal ID="ltlGWRevisedDate" runat="server" /></p>
                    <asp:Literal ID="ltlGWMeas" runat="server" />
                    <br /><br />
                    <h5 class="sectionHeadings">Measurement Specific Job Hazards</h5><br />
                    <asp:ListView ID="lvGWMeasJHA" runat="server" DataKeyNames="site_jha_id">
                        <LayoutTemplate>
                            <div id="itemPlaceholder" runat="server" />
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div id="Div1" runat="server" style="border:1px solid #999999;padding:5px 5px 5px 5px;">
                                <p style="font-weight:bold;margin-top:0px;"><%# Eval("jha_description")%></p>
                                <asp:ListView ID="lvGWMeasRemarks" runat="server" DataSource='<%# getRemarksForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_specificcond_id">
                                    <LayoutTemplate>
                                        <ul>
                                            <li runat="server" id="itemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li><%# Eval("remarks")%></li>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <span class="noData">No remarks have been entered.</span>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                                <div style="padding-left:30px;">
                                    <p style="font-weight:bold;margin-bottom:0px;">Know Your Limits</p>
                                    <asp:ListView ID="lvGWMeasOpLimits" runat="server" DataSource='<%# getJobOpLimitsForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_reflevel_id">
                                        <LayoutTemplate>
                                            <table cellpadding="3" style="border: 1px solid #cccccc;width:100%;">
                                                <tr>
                                                    <th style="font-size:smaller;width:150px;">Limit Type</th>
                                                    <th style="font-size:smaller;width:100px;">Limit Value</th>
                                                    <th style="font-size:smaller;">Remarks</th>
                                                </tr>
                                                <tr runat="server" id="itemPlaceholder" />
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr id="Tr1" runat="server">
                                                <td><%# Eval("reflevel_tp")%>&nbsp;</td>
                                                <td align="center"><%# Eval("reflevel_va")%>&nbsp;</td>
                                                <td><%# Eval("remarks")%>&nbsp;</td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <span class="noData">No job operational limits have been entered.</span>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p>No site specific job hazards have been entered.</p>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </asp:Panel>
            <br /><br />
            <asp:Panel ID="pnlQWMeas" runat="server" Visible="false">
                <h4 class="sectionHeadings">WATER QUALITY MEASUREMENTS</h4>
                <div style="padding-left:10px;">
                    <p style="font-size:8pt;margin-top:5px;">Revised by: <asp:Literal ID="ltlQWRevisedBy" runat="server" /> &nbsp;&nbsp;&nbsp; Date revised: <asp:Literal ID="ltlQWRevisedDate" runat="server" /></p>
                    <asp:Literal ID="ltlQWMeas" runat="server" />
                    <br /><br />
                    <h5 class="sectionHeadings">Measurement Specific Job Hazards</h5><br />
                    <asp:ListView ID="lvQWMeasJHA" runat="server" DataKeyNames="site_jha_id">
                        <LayoutTemplate>
                            <div id="itemPlaceholder" runat="server" />
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div id="Div1" runat="server" style="border:1px solid #999999;padding:5px 5px 5px 5px;">
                                <p style="font-weight:bold;margin-top:0px;"><%# Eval("jha_description")%></p>
                                <asp:ListView ID="lvQWMeasRemarks" runat="server" DataSource='<%# getRemarksForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_specificcond_id">
                                    <LayoutTemplate>
                                        <ul>
                                            <li runat="server" id="itemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li><%# Eval("remarks")%></li>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <span class="noData">No remarks have been entered.</span>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                                <div style="padding-left:30px;">
                                    <p style="font-weight:bold;margin-bottom:0px;">Know Your Limits</p>
                                    <asp:ListView ID="lvQWMeasOpLimits" runat="server" DataSource='<%# getJobOpLimitsForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_reflevel_id">
                                        <LayoutTemplate>
                                            <table cellpadding="3" style="border: 1px solid #cccccc;width:100%;">
                                                <tr>
                                                    <th style="font-size:smaller;width:150px;">Limit Type</th>
                                                    <th style="font-size:smaller;width:100px;">Limit Value</th>
                                                    <th style="font-size:smaller;">Remarks</th>
                                                </tr>
                                                <tr runat="server" id="itemPlaceholder" />
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr id="Tr1" runat="server">
                                                <td><%# Eval("reflevel_tp")%>&nbsp;</td>
                                                <td align="center"><%# Eval("reflevel_va")%>&nbsp;</td>
                                                <td><%# Eval("remarks")%>&nbsp;</td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <span class="noData">No job operational limits have been entered.</span>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p>No activity specific job hazards have been entered.</p>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </asp:Panel>
            <br /><br />
            <asp:Panel ID="pnlLakeMeas" runat="server" Visible="false">
                <h4 class="sectionHeadings">LAKE / RESERVOIR MEASUREMENTS</h4>
                <div style="padding-left:10px;">
                    <p style="font-size:8pt;margin-top:5px;">Revised by: <asp:Literal ID="ltlLakeRevisedBy" runat="server" /> &nbsp;&nbsp;&nbsp; Date revised: <asp:Literal ID="ltlLakeRevisedDate" runat="server" /></p>
                    <asp:Literal ID="ltlLakeMeas" runat="server" />
                    <br /><br />
                    <h5 class="sectionHeadings">Measurement Specific Job Hazards</h5><br />
                    <asp:ListView ID="lvLakeMeasJHA" runat="server" DataKeyNames="site_jha_id">
                        <LayoutTemplate>
                            <div id="itemPlaceholder" runat="server" />
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div id="Div1" runat="server" style="border:1px solid #999999;padding:5px 5px 5px 5px;">
                                <p style="font-weight:bold;margin-top:0px;"><%# Eval("jha_description")%></p>
                                <asp:ListView ID="lvLakeMeasRemarks" runat="server" DataSource='<%# getRemarksForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_specificcond_id">
                                    <LayoutTemplate>
                                        <ul>
                                            <li runat="server" id="itemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li><%# Eval("remarks")%></li>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <span class="noData">No remarks have been entered.</span>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                                <div style="padding-left:30px;">
                                    <p style="font-weight:bold;margin-bottom:0px;">Know Your Limits</p>
                                    <asp:ListView ID="lvLakeMeasOpLimits" runat="server" DataSource='<%# getJobOpLimitsForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_reflevel_id">
                                        <LayoutTemplate>
                                            <table cellpadding="3" style="border: 1px solid #cccccc;width:100%;">
                                                <tr>
                                                    <th style="font-size:smaller;width:150px;">Limit Type</th>
                                                    <th style="font-size:smaller;width:100px;">Limit Value</th>
                                                    <th style="font-size:smaller;">Remarks</th>
                                                </tr>
                                                <tr runat="server" id="itemPlaceholder" />
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr id="Tr1" runat="server">
                                                <td><%# Eval("reflevel_tp")%>&nbsp;</td>
                                                <td align="center"><%# Eval("reflevel_va")%>&nbsp;</td>
                                                <td><%# Eval("remarks")%>&nbsp;</td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <span class="noData">No job operational limits have been entered.</span>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p>No site specific job hazards have been entered.</p>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </asp:Panel>
            <br /><br />
            <asp:Panel ID="pnlEcoMeas" runat="server" Visible="false">
                <h4 class="sectionHeadings">ECOLOGICAL MEASUREMENTS</h4>
                <div style="padding-left:10px;">
                    <p style="font-size:8pt;margin-top:5px;">Revised by: <asp:Literal ID="ltlEcoRevisedBy" runat="server" /> &nbsp;&nbsp;&nbsp; Date revised: <asp:Literal ID="ltlEcoRevisedDate" runat="server" /></p>
                    <asp:Literal ID="ltlEcoMeas" runat="server" />
                    <br /><br />
                    <h5 class="sectionHeadings">Measurement Specific Job Hazards</h5><br />
                    <asp:ListView ID="lvEcoMeasJHA" runat="server" DataKeyNames="site_jha_id">
                        <LayoutTemplate>
                            <div id="itemPlaceholder" runat="server" />
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div id="Div1" runat="server" style="border:1px solid #999999;padding:5px 5px 5px 5px;">
                                <p style="font-weight:bold;margin-top:0px;"><%# Eval("jha_description")%></p>
                                <asp:ListView ID="lvEcoMeasRemarks" runat="server" DataSource='<%# getRemarksForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_specificcond_id">
                                    <LayoutTemplate>
                                        <ul>
                                            <li runat="server" id="itemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li><%# Eval("remarks")%></li>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <span class="noData">No remarks have been entered.</span>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                                <div style="padding-left:30px;">
                                    <p style="font-weight:bold;margin-bottom:0px;">Know Your Limits</p>
                                    <asp:ListView ID="lvEcoMeasOpLimits" runat="server" DataSource='<%# getJobOpLimitsForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_reflevel_id">
                                        <LayoutTemplate>
                                            <table cellpadding="3" style="border: 1px solid #cccccc;width:100%;">
                                                <tr>
                                                    <th style="font-size:smaller;width:150px;">Limit Type</th>
                                                    <th style="font-size:smaller;width:100px;">Limit Value</th>
                                                    <th style="font-size:smaller;">Remarks</th>
                                                </tr>
                                                <tr runat="server" id="itemPlaceholder" />
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr id="Tr1" runat="server">
                                                <td><%# Eval("reflevel_tp")%>&nbsp;</td>
                                                <td align="center"><%# Eval("reflevel_va")%>&nbsp;</td>
                                                <td><%# Eval("remarks")%>&nbsp;</td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <span class="noData">No job operational limits have been entered.</span>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p>No site specific job hazards have been entered.</p>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </asp:Panel>
            <br /><br />
            <asp:Panel ID="pnlAtmMeas" runat="server" Visible="false">
                <h4 class="sectionHeadings">ATMOSPHERIC MEASUREMENTS</h4>
                <div style="padding-left:10px;">
                    <p style="font-size:8pt;margin-top:5px;">Revised by: <asp:Literal ID="ltlAtmRevisedBy" runat="server" /> &nbsp;&nbsp;&nbsp; Date revised: <asp:Literal ID="ltlAtmRevisedDate" runat="server" /></p>
                    <asp:Literal ID="ltlAtmMeas" runat="server" />
                    <br /><br />
                    <h5 class="sectionHeadings">Measurement Specific Job Hazards</h5><br />
                    <asp:ListView ID="lvAtmMeasJHA" runat="server" DataKeyNames="site_jha_id">
                        <LayoutTemplate>
                            <div id="itemPlaceholder" runat="server" />
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div id="Div1" runat="server" style="border:1px solid #999999;padding:5px 5px 5px 5px;">
                                <p style="font-weight:bold;margin-top:0px;"><%# Eval("jha_description")%></p>
                                <asp:ListView ID="lvAtmMeasRemarks" runat="server" DataSource='<%# getRemarksForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_specificcond_id">
                                    <LayoutTemplate>
                                        <ul>
                                            <li runat="server" id="itemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li><%# Eval("remarks")%></li>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <span class="noData">No remarks have been entered.</span>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                                <div style="padding-left:30px;">
                                    <p style="font-weight:bold;margin-bottom:0px;">Know Your Limits</p>
                                    <asp:ListView ID="lvAtmMeasOpLimits" runat="server" DataSource='<%# getJobOpLimitsForJHA(Eval("site_jha_id")) %>' DataKeyNames="site_reflevel_id">
                                        <LayoutTemplate>
                                            <table cellpadding="3" style="border: 1px solid #cccccc;width:100%;">
                                                <tr>
                                                    <th style="font-size:smaller;width:150px;">Limit Type</th>
                                                    <th style="font-size:smaller;width:100px;">Limit Value</th>
                                                    <th style="font-size:smaller;">Remarks</th>
                                                </tr>
                                                <tr runat="server" id="itemPlaceholder" />
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr id="Tr1" runat="server">
                                                <td><%# Eval("reflevel_tp")%>&nbsp;</td>
                                                <td align="center"><%# Eval("reflevel_va")%>&nbsp;</td>
                                                <td><%# Eval("remarks")%>&nbsp;</td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <span class="noData">No job operational limits have been entered.</span>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <p>No site specific job hazards have been entered.</p>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </asp:Panel>
        </div>
        <br />
        <hr />

        <!-- ADMINISTRATION -->
        <div style="width:100%;font-size:8pt;">
            <div style="float:left;width:49%;">
                <p style="margin:0px;">Reviewed By: <asp:Literal ID="ltlReviewedBy" runat="server" /></p>
                <p style="margin:0px;">Last Reviewed Date: <asp:Literal ID="ltlReviewedDate" runat="server" /></p>
                <p style="margin:0px;">Reviewer Comments: <asp:Literal ID="ltlReviewerComments" runat="server" /></p>
            </div>
            <div style="float:right;width:49%;">
                <p style="margin:0px;">Approved By: <asp:Literal ID="ltlApprovedBy" runat="server" /></p>
                <p style="margin:0px;">Last Approved Date: <asp:Literal ID="ltlApprovedDate" runat="server" /></p>
            </div>
        </div>
    </asp:Panel>
    </div>
    </form>
</body>
</html>
