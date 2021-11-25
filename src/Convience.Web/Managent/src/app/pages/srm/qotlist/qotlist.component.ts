//import { Component, OnInit } from '@angular/core';
import { GridOptions } from 'ag-grid-community';
import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";
import { FormBuilder, FormGroup } from '@angular/forms';
import { SrmQotService } from 'src/app/business/srm/srm-qot.service';
import { StorageService } from '../../../services/storage.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LayoutComponent } from '../../layout/layout/layout.component';


@Component({
  selector: 'app-qotlist',
  templateUrl: './qotlist.component.html',
  styleUrls: ['./qotlist.component.less']
})

export class QotlistComponent implements OnInit {
  qotid;
  rowDataQot;
  columnDefs;
  //rowData: [];
  //columnApi;
  //gridApi;
  form_searchQot: FormGroup = new FormGroup({});
  page: number = 1;//??
  size: number = 2;//??
  total: number;
  detailCellRendererParams;
  detailCellRendererParams2;
  defaultColDef;
  rowSelection = "single";
  /**/
  public gridApi;
  rowData_Qot;
  columnApi;
  QotH: any;
  constructor(
    private _formBuilder: FormBuilder,
    private http: HttpClient,
    private _srmQotService: SrmQotService, 
    private _storageService: StorageService,    
    private activatedRoute: ActivatedRoute,
    private _layout: LayoutComponent,
    private _router: Router,
  )
  {
    //this.activatedRoute.params.subscribe((params) => this.qotid = params['id']);

    this.columnDefs = [
     /* { field:'序號', resizable: true,cellRenderer: 'agGroupCellRenderer',},
      { field: '詢價單號', resizable: true},
      { field: '狀態', resizable: true},
      { field: '建立日期', resizable: true },
      { field: '建立人員', resizable: true},
      { field: '最後異動日期', resizable: true},
      { field: '最後異動人員', resizable: true },*/
      //{  headerName: "Row",valueGetter: "node.rowIndex + 1"},
      //{  headerName:'序號',field: 'VRfqId', resizable: true,cellRenderer: 'agGroupCellRenderer',},    
      {  headerName:'序號',valueGetter: "node.rowIndex + 1", resizable: true,cellRenderer: 'agGroupCellRenderer',},    
      //{  headerName:'狀態',field: 'VStatus', valueFormatter:'switch(value){case 1 : return "初始"; case 3 : return "接收"; case 5 : return "確認";case 7 : return "啟動"; case 18 : return "完成";default : return "未知";}'},
      {  headerName:'狀態',field: 'VStatusDesc'},
      {  headerName:'詢價單號',field: 'VRfqNum', resizable: true},
      {  headerName:'建立日期',field: 'VCreateDate', resizable: true,valueFormatter:dateFormatter },
      {  headerName:'建立人員',field: 'VCreateBy', resizable: true},
      {  headerName:'最後異動日期',field: 'VLastUpdateDate', resizable: true,valueFormatter:dateFormatter },
      //{  headerName:'最後異動人員',field: 'VLastUpdateBy', resizable: true },

    ];
    this.detailCellRendererParams = {
      detailGridOptions: {
        columnDefs: [
          /*{ field: '序號' },
          { field: '報價單號' },
          { field: '狀態', minWidth: 150 },
          { field: '料號' },
          { field: '建立日期', minWidth: 150 },
          { field: '建立人員', minWidth: 150 },
          { field: '最後異動日期', minWidth: 150 },
          { field: '最後異動人員', minWidth: 150 },*/
          //{ headerName:'序號',field: 'QQotId' },
          { headerName: '操作', field: 'fieldName',
          cellRenderer : function(params){
            var eDiv = document.createElement('div');
              eDiv.innerHTML = '<span class="my-css-class"><button nz-button nzType="primary" class="btn-simple" style="height:39px">檢視</button></span>';
              var eButton = eDiv.querySelectorAll('.btn-simple')[0];  
              eButton.addEventListener('click', function() {
               //alert( params.data.QVendorId)
               //window.open('../srm/qot?id=' + params.data.QQotId + '&rfqid=' + params.data.QRfqId+ '&vendorid=' + params.data.QVendorId);
               window.open('../srm/qot?id=' + params.data.QQotId + '&rfqid=' + params.data.QRfqId+ '&vendorid=' + params.data.QVendorId);
               //this._layout.navigateTo('qot'); //???進不去
               //this._router.navigate(['srm/qot', { id: params.data.QQotId }]);
              });
              return eDiv;
              }
          },
          {  headerName: "QVendorId",field: 'QVendorId',hide:"true"},
          {  headerName: "QRfqId",field: 'QRfqId',hide:"true"},
          {  headerName: "QQotId",field: 'QQotId',hide:"true"},
          {  headerName: "序號",valueGetter: "node.rowIndex + 1"},
          {  headerName:'報價單號',field: 'QQotNum' },
          {  headerName:'狀態',field: 'QStatusDesc', minWidth: 150 },
          {  headerName:'料號',field: 'QMatnr'},
          {  headerName:'建立日期',field: 'QCreateDate', minWidth: 150 ,valueFormatter:dateFormatter },
          {  headerName:'建立人員',field: 'QCreateBy', minWidth: 150 },
          {  headerName:'最後異動日期',field: 'QLastUpdateDate', minWidth: 150,valueFormatter:dateFormatter  },
          {  headerName:'最後異動人員',field: 'QLastUpdateBy', minWidth: 150 },
        ],
        defaultColDef: {
          flex: 1,
        },
      },
        getDetailRowData: params => {
          params.successCallback(params.data.SrmQotHs);
        }

    }
  }
  ngOnInit(): void {
    console.log('username ='+this._storageService.userName);
    //查出全部待辦??? "詢價狀態不為確認"
    // throw new Error('Method not implemented.');
    this.form_searchQot = this._formBuilder.group({
      rfqno: [null],
      qotstatus: 1,
      qotmatnr:[null],
      //??
      //vendor:"2"
      vendor:this._storageService.userName
    });
    this.refresh();
  }
  searchQOT() {
    this.refresh();
    //依搜尋條件查詢???
    //料號、報價單狀態
  }
  refresh() {
    var query = {

      RFQ_NUM: this.form_searchQot.value["rfqno"] == null ? "" : this.form_searchQot.value["rfqno"],
      MATNR: this.form_searchQot.value["qotmatnr"] == null ? "" : this.form_searchQot.value["qotmatnr"],
      STATUS: this.form_searchQot.value["qotstatus"] == null ? "0" : this.form_searchQot.value["qotstatus"],
      //????待vendor
      //getVendorId()
      //VENDOR:2
      vendor:this._storageService.userName
    }
    this.getOotList(query);
  }
  pageChange() {
    this.refresh();
  }

  /*open(id) {
    window.open('../srm/qot?id=' + id);
  }*/
  //???供應商登入用VENDERCODE 
  getVendorId()
  {

  }
  getOotList(query){
    if(query==null)
    {
      query = {
        RFQ_NUM: "",
        MATNR: "",
        STATUS: "",
        VENDOR:"", 
        page: this.page,
        size: this.size
      }
    }

    //???等function 寫好打開
    console.info(query)
    this._srmQotService.GetQotList(query)
       .subscribe((result) => {
         this.rowDataQot = result;
         console.info(result);
       });
  }

  // gridOptions = {
  //   columnDefs:this.columnDefs,
  //   masterDetail: true,
  //   rowSelection: 'multiple',
  //   suppressRowClickSelection: true,
  //   enableRangeSelection: true,
  //   pagination: true,
  //   paginationAutoPageSize: true,
  //   detailCellRendererParams: {
  //     detailGridOptions: {
  //       columnDefs1: [
  //         { field: '報價單號' },
  //         { field: '報價單號' },
  //         { field: '狀態', minWidth: 150 },
  //         { field: '料號' },
  //         { field: '建立日期', minWidth: 150 },
  //         { field: '建立人員', minWidth: 150 },
  //         { field: '最後異動日期', minWidth: 150 },
  //         { field: '最後異動人員', minWidth: 150 },
  //       ],
  //       defaultColDef: {
  //         flex: 1,
  //       },
  //       getDetailRowData: params => {
  //         params.successCallback(params.data.detail);
  //     }
  //     },

  //   },
  // }

  rowData = [
    { 序號: '1',  詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'A', 最後異動日期: '2021/07/14', 最後異動人員: 'A', 狀態: '啟動' ,
  detail:[{報價單號:'1',狀態:'1',料號:'1',建立日期:'1',建立人員:'1',最後異動日期:'1',最後異動人員:'1'}]},
    { 序號: '2',  詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'B', 最後異動日期: '2021/07/14', 最後異動人員: 'B', 狀態: '啟動' ,
    detail:[{報價單號:'2',狀態:'2',料號:'2',建立日期:'2',建立人員:'2',最後異動日期:'2',最後異動人員:'2'}]},
    { 序號: '3',  詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'C', 最後異動日期: '2021/07/14', 最後異動人員: 'C', 狀態: '啟動' ,
    detail:[{報價單號:'1',狀態:'1',料號:'1',建立日期:'1',建立人員:'1',最後異動日期:'1',最後異動人員:'1'}]},
    { 序號: '4',  詢價單號: 'RFQ0000001', 建立日期: '2021/07/14', 建立人員: 'D', 最後異動日期: '2021/07/14', 最後異動人員: 'D', 狀態: '啟動' ,
    detail:[{報價單號:'1',狀態:'1',料號:'1',建立日期:'1',建立人員:'1',最後異動日期:'1',最後異動人員:'1'}]},
  ];
 

  private selectedRows = [];

  //onRowClicked() {
  //  alert(1);
  //}
  onRowClicked(event) {
  //event.data The selected row data, event.event is the mouse event, etc. You can log to see for yourself
    console.log('a row was clicked', event);
    //alert(event.data["報價單號"])
    //alert(event.data[0].序號)
    //var itxst = JSON.stringify(event.data);
    //alert(itxst);
  }
  onGridReady(params) {
    this.gridApi = params.api;
    this.columnApi = params.columnApi;
    this.gridApi.sizeColumnsToFit();
    this.getOotList(null);
  }
//   gridOptions1 = {
//     // enable Master / Detail
//     masterDetail: true,

//     // the first Column is configured to use agGroupCellRenderer
//     columnDefs: [
//         { field: 'name', cellRenderer: 'agGroupCellRenderer' },
//         { field: 'account' }
//     ],

//     // provide Detail Cell Renderer Params
//     detailCellRendererParams: {
//         // provide the Grid Options to use on the Detail Grid
//         detailGridOptions: {
//             columnDefs: [
//                 { field: 'callId' },
//                 { field: 'direction' },
//                 { field: 'number'}
//             ]
//         },
//         // get the rows for each Detail Grid
//         getDetailRowData: params => {
//             params.successCallback(params.data.callRecords);
//         }
//     },

//     // other grid options ...
// }
/*open(id) {
  //alert(id.data["VRfqNum"])
  console.info('qoooooooooooooooooooooooooooooooooot='+id.data)
  this._layout.navigateTo('qot');
  this._router.navigate(['srm/qot', { id: id }]);
  //window.open('../srm/rfq?id=' + id);
}*/

}
function dateFormatter(data) {
  if(data.value==null) return "";
  var date=new Date(data.value);
  console.log(date.getHours())
  return `${date.getFullYear()}/${date.getMonth()+1}/${date.getDate()} ${date.getHours()}:${date.getUTCMinutes()}:${date.getSeconds()}`;
}