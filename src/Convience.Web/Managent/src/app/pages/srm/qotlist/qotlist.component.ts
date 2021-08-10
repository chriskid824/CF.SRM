//import { Component, OnInit } from '@angular/core';
import { GridOptions } from 'ag-grid-community';
import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";
import { FormBuilder, FormGroup } from '@angular/forms';
import { SrmQotService } from 'src/app/business/srm/srm-qot.service';

@Component({
  selector: 'app-qotlist',
  templateUrl: './qotlist.component.html',
  styleUrls: ['./qotlist.component.less']
})

export class QotlistComponent implements OnInit {
  rowDataQot;
  columnDefs;
  //rowData: [];
  //columnApi;
  //gridApi;
  form_searchQot: FormGroup = new FormGroup({});
  page: number = 1;//??
  size: number = 2;//??
  detailCellRendererParams;
  detailCellRendererParams2;
  defaultColDef;
  rowSelection = "multiple";
  /**/
  public gridApi;
  rowData_Qot;
  columnApi;
  constructor(
    private _formBuilder: FormBuilder,private http: HttpClient,private _srmQotService: SrmQotService
  )
  {
    this.columnDefs = [
     /* { field:'序號', resizable: true,cellRenderer: 'agGroupCellRenderer',},
      { field: '詢價單號', resizable: true},
      { field: '狀態', resizable: true},
      { field: '建立日期', resizable: true },
      { field: '建立人員', resizable: true},
      { field: '最後異動日期', resizable: true},
      { field: '最後異動人員', resizable: true },*/

      {  headerName:'序號',field: 'RFQ_ID', resizable: true,cellRenderer: 'agGroupCellRenderer',},
      {  headerName:'詢價單號',field: 'RFQ_NUM', resizable: true},
      {  headerName:'狀態',field: 'STATUS', resizable: true},
      {  headerName:'建立日期',field: 'CREATE_DATE', resizable: true },
      {  headerName:'建立人員',field: 'CREATE_BY', resizable: true},
      {  headerName:'最後異動日期',field: 'LAST_UPDATE_DATE', resizable: true},
      {  headerName:'最後異動人員',field: 'LAST_UPDATE_BY', resizable: true },

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
          { headerName:'序號',field: 'QOT_ID' },
          { headerName:'報價單號',field: 'QOT_NUM' },
          { headerName:'狀態',field: 'STATUS', minWidth: 150 },
          { headerName:'料號',field: 'MATNR'},
          { headerName:'建立日期',field: 'CREATE_DATE', minWidth: 150 },
          { headerName:'建立人員',field: 'CREATE_BY', minWidth: 150 },
          { headerName:'最後異動日期',field: 'LAST_UPDATE_DATE', minWidth: 150 },
          { headerName:'最後異動人員',field: 'LAST_UPDATE_BY', minWidth: 150 },
          
        ],
        defaultColDef: {
          flex: 1,
        },
      },
        getDetailRowData: params => {
          params.successCallback(params.data.detail);
        }

    }
  }
  ngOnInit(): void {
    //查出全部待辦??? "詢價狀態不為確認"
    // throw new Error('Method not implemented.');
    this.form_searchQot = this._formBuilder.group({
      rfqno: [null],
      qotstatus: [1],
      qotmatnr:[null]
    });
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
      STATUS: this.form_searchQot.value["qotstatus"] == null ? "0" : this.form_searchQot.value["qotstatus"]
    }
    this.getOotList(query);
  }
  getOotList(query){
    if(query==null)
    {
      query = {
        RFQ_NUM: "",
        MATNR: "",
        STATUS: "0",
        page: this.page,
        size: this.size
      }
    }
    //???等function 寫好打開
    this._srmQotService.GetQotList(query)
       .subscribe((result) => {
         this.rowDataQot = result;
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
    alert(event.data["報價單號"])
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

}
