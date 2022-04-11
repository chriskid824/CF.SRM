import { Component, OnInit,ViewEncapsulation,ViewChild,TemplateRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NzDatePickerComponent} from 'ng-zorro-antd/date-picker';
import datepickerFactory from 'jquery-datepicker';
import datepickerJAFactory from 'jquery-datepicker/i18n/jquery.ui.datepicker-en-GB';
import { AgGridDatePickerComponent} from './AGGridDatePickerCompponent';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ViewSrmFileUploadRecordH,ViewSrmFileUploadRecordL } from '../model/File';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";
import { EditButtonComponent } from './button-cell-renderer.component';
import { FileModalComponent } from '../file-modal/file-modal.component';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { SrmFileService } from 'src/app/business/srm/srm-file.service';
import { FileService } from 'src/app/business/file.service';
import { FileInfo } from '../../content-manage/model/fileInfo';
import { StorageService } from 'src/app/services/storage.service';
import { PoDateModalComponent} from './po-date-modal';
import { formatDate } from '@angular/common';
import { ConditionalExpr } from '@angular/compiler';
// import { TotalValueRenderer } from './total-value-renderer.component';
declare const $: any; // avoid the error on $(this.eInput).datepicker();
datepickerFactory($);
datepickerJAFactory($);
@Component({
  selector: 'app-po',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './po.component.html',
  styleUrls: ['./po.component.less']
})
export class PoComponent implements OnInit {
   @ViewChild('filemodal')
   filemodal: FileModalComponent;

  // @ViewChild('ctest2')
  // ctest2: TemplateRef<any>;
  isvendor:boolean=false;
  gridApi;
  gridColumnApi;
  columnDefs;
  defaultColDef;
  detailCellRendererParams;
  components;
  rowData: any;
  frameworkComponents: any;
  searchForm: FormGroup = new FormGroup({});
  page: number = 1;
  size: number = 2;
  total: number;
  isVisible=false;
  searchId;
  tplModal: NzModalRef;
  fileList:any[] = [];
  fileNameList:any[] = [];
  uploading: boolean = false;
  formData:FormData;
  poData:any;
  polData:any;
  showUploadList = {
    showDownloadIcon: true,
   showRemoveIcon: false,
   };
  fileRecord: ViewSrmFileUploadRecordH = {recordHId:1,templateId:1100,srmFileuploadRecordL:[{recordLId:1,recordHId:1,filename:"test1",filetypename:"SIP"},{recordLId:2,recordHId:1,filename:"test2",filetypename:"SOP"},{recordLId:3,recordHId:1,filename:"test3",filetypename:"第三方檢驗文件"}]};
  constructor(private _formBuilder: FormBuilder,private http: HttpClient,private _srmPoService: SrmPoService
    ,private _messageService: NzMessageService,
    private _modalService: NzModalService,
    private _srmFileService: SrmFileService,
    private _fileService: FileService,
    private dialog: MatDialog,private route: ActivatedRoute,
    private _storageService: StorageService) {
      this.route.params.subscribe(params => {
        this.searchId = params['id'];
        console.log(params['id']);
      });
      this.frameworkComponents = {
        buttonRenderer: EditButtonComponent,
      }
    this.columnDefs = [
      {
        headerName:'採購單識別碼',
        field: 'PoId',
        hide:'true',
      },
      {
        headerName:'單號',
        field: 'PoNum',
        cellRenderer: 'agGroupCellRenderer',
      },
      {
        headerName:'供應商識別碼',
        field: 'VendorId',
        hide:'true',
      },
      {
        headerName:'供應商',
        field: 'VendorName',
      },
      {
        headerName:'狀態',
        field: 'StatusDesc',
      },
      {
        headerName:'總金額',
        field: 'TotalAmount',
      },
      {
        headerName:'採購員',
        field: 'EkgryDesc',
      },
      {
        headerName:'廠別',
        field: 'Org',
        valueGetter: function (params) {
          console.info(params);
          if(params.data.Org=='3100')
          {
            return "千附精密";
          }
          else if(params.data.Org=='1200')
          {
            return "機械事業處";
          }
          else if(params.data.Org=='1100')
          {
            return "廠工事業處";
          }
          return params.data.Org;
        }
      },
      {
        headerName:'文件日期',
        field: 'DocDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'廠商接收日期',
        field: 'ReplyDate',
        valueFormatter:dateFormatter,
        editable: function(params){return true},
        cellEditorFramework: AgGridDatePickerComponent
      },
      {
        headerName:'公司',
        field: 'Org',
        valueGetter: function (params) {
          console.info(params);
          if(params.data.Org=='3100')
          {
            return "千附精密";
          }
          return "千附實業";
        }
      },
      {
        headerName:'拋轉日期',
        field: 'CreateDate',
        valueFormatter:dateFormatter
      },
      { headerName: '操作', field: 'fieldName',
      cellRenderer : function(params){
        if(params.data.Status==21)
        {
          var eDiv = document.createElement('div');
          eDiv.innerHTML = '<span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple" style="height:39px">接收</button></span>';
          var eButton = eDiv.querySelectorAll('.btn-simple')[0];
          eButton.addEventListener('click', function() {
            _srmPoService.UpdateStatus(params.data.PoId).subscribe(result=>{
              alert('採購單號:'+params.data.PoNum+'已接收');
              params.data.Status=15;
              params.data.ReplyDate=new Date();
              params.data.StatusDesc="待交貨";
              params.data.SrmPoLs.forEach(element => {
                if(element.Status!=12)
                {
                  element.Status=15;
                  element.StatusDesc="待交貨";
                  element.ReplyDeliveryDate=element.DeliveryDate;
                }
              });
              eDiv.innerHTML='';
              params.api.refreshCells();
              params.api.redrawRows();
            });
            });
          // if(params.data.hasFile)
          // {
          //   eDiv.innerHTML = '<span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple" style="height:39px">接收</button></span><span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple2" style="height:39px">檔案</button></span>';
          //   var eButton = eDiv.querySelectorAll('.btn-simple')[0];
          //   eButton.addEventListener('click', function() {
          //     _srmPoService.UpdateStatus(params.data.PoId).subscribe(result=>{
          //       alert('採購單號:'+params.data.PoNum+'已接收');
          //       params.data.Status=11;
          //       params.data.ReplyDate=new Date();
          //       params.data.StatusDesc="待回覆";
          //       params.data.SrmPoLs.forEach(element => {
          //         element.Status=11;
          //         element.StatusDesc="待回覆";
          //         eDiv.innerHTML='';
          //       });
          //       params.api.refreshCells();
          //     });
          //   });
          // }
          // else
          // {
          //   eDiv.innerHTML = '<span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple2" style="height:39px">檔案</button></span>';
          // }

          // var eButton_file = eDiv.querySelectorAll('.btn-simple2')[0];

          // eButton_file.addEventListener('click', function() {
          //   params.ondblclick(params);
          // });
          return eDiv;
          }

          // else if(params.data.Status==11||params.data.Status==15)
          // {
          //   var eDiv = document.createElement('div');
          //   eDiv.innerHTML = '<span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple" style="height:39px">統一回覆</button></span>';
          //   var eButton = eDiv.querySelectorAll('.btn-simple')[0];

          //   eButton.addEventListener('click', function() {
          //     const dialogConfig = new MatDialogConfig();
          //     dialogConfig.disableClose = true;
          //     dialogConfig.autoFocus = true;
          //     dialogConfig.maxHeight = "1500px";
          //     dialogConfig.data = params.data.PoNum;
          //     dialog.open(PoDateModalComponent, dialogConfig);
          //     });
          //   return eDiv;
          //   }
        },
        cellRendererParams: {
          // ondblclick:this.openFileModal.bind(this),
          label: '',
        },
      }
    ];
    this.defaultColDef = {
      editable: false,
      enableRowGroup: true,
      enablePivot: true,
      enableValue: true,
      sortable: true,
      resizable: true,
      filter: true,
      flex: 1,
      minWidth: 150,
    };
    this.components = { datePicker: getDatePicker()};
    this.detailCellRendererParams = {
      refreshStrategy: 'rows',
      detailGridOptions: {
        frameworkComponents:this.frameworkComponents,
        rowStyle: { background: 'beige' },
        columnDefs: [
          { headerName: '操作',
          //           cellRenderer : function(params){
          //               var eDiv = document.createElement('div');
          //               eDiv.innerHTML = '<span class="my-css-class"><button nz-button nzType="primary" class="btn-simple" style="height:39px">編輯檔案</button></span>';
          //               var eButton = eDiv.querySelectorAll('.btn-simple')[0];

          //               eButton.addEventListener('click', function() {
          // this.isVisible=true;

          //               });
          //               return eDiv;

          //             }
                       cellRenderer : function(params){
                          var eDiv = document.createElement('div');
                          if(params.data.Status==11)
                          {
                            eDiv.innerHTML = '<span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple" style="height:39px">檔案</button></span><span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple" style="height:39px">回覆</button></span>';
                          }
                          else
                          {
                            eDiv.innerHTML = '<span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple" style="height:39px">檔案</button></span>';
                          }
                          var eButton_file = eDiv.querySelectorAll('.btn-simple')[0];
                          eButton_file.addEventListener('click', function() {
                            params.ondblclick(params);
                          });
                          if(params.data.Status==11)
                          {
                            var eButton_reply = eDiv.querySelectorAll('.btn-simple')[1];
                            eButton_reply.addEventListener('click', function() {
                              console.info(params.data.PoId);
                              console.info(params.data.PoLId);
                              _srmPoService.UpdateStatus_Reply(params.data.PoId,params.data.PoLId).subscribe(result=>{
                                alert('採購單:'+params.data.PoNum+'-'+params.data.PoLId+' 已回覆');
                                params.data.Status=15;
                                params.data.ReplyDate=new Date();
                                params.data.StatusDesc="待交貨";
                                // params.data.SrmPoLs.forEach(element => {
                                //   element.Status=15;
                                //   element.StatusDesc="待交貨";
                                //   element.ReplyDeliveryDate=element.DeliveryDate;
                                //   eDiv.innerHTML='';
                                // });
                                params.api.refreshCells();
                                // params.api.redrawRows();
                              });
                            });
                          }

                          return eDiv;

                        },
                        cellRendererParams: {
                          ondblclick:this.openFileModal.bind(this),
                          label: '',
                        },
                      //  cellRenderer: 'buttonRenderer',
                      //  cellRendererParams: {
                      //   ondblclick:this.start.bind(this),
                      //   label: '',
                      // },
                      // pinned: 'left',
                    },
          {
            headerName:'項次',
            field: 'PoLId',
          },
          {
            headerName:'採購單號',
            field: 'PoNum',
            hide:'true',
          },
          {
            headerName:'料號',
            field: 'Matnr',
          },
          {
            headerName:'物料內文',
            field: 'Description',
          },
          {
            headerName:'採單數量',
            field: 'Qty',
          },
          {
            headerName:'單價',
            field: 'Price',
          },
          {
            headerName:'原始需求日',
            field: 'OriginalDate',
            valueFormatter:dateFormatter
          },
          {
            headerName:'本次需求日',
            field: 'DeliveryDate',
            valueFormatter:dateFormatter
          },
          {
            headerName:'廠商交貨日期',
            field: 'ReplyDeliveryDate',
            editable: function(params){console.info(params.node.parent);return true},
            valueFormatter:dateFormatter,
            stopEditingWhenCellsLoseFocus:false,
            // valueFormatter: this.expiryDateFormatter,
            cellEditorFramework: AgGridDatePickerComponent,
            cellClassRules: {
              //'rag-green': 'x == null',
              'rag-yellow': 'x == null',
              //'rag-red': 'x == 21',
            },
          },
          {
            headerName:'狀態',
            field: 'StatusDesc',
          },
          // {
          //   headerName:'狀態',
          //   field: 'Status',
          //   cellClassRules: {
          //     'rag-green': 'x == 12',
          //     'rag-amber': 'x == 11',
          //     'rag-red': 'x == 21',
          //   },
          //   valueFormatter:'switch(value){case 21 : return "待接收"; case 11 : return "已接收"; case 12 : return "已回覆";case 14 : return "已交貨";case 15 : return "待交貨"; default : return "未知";}'
          // },
          {
            headerName:'交貨地點',
            field: 'DeliveryPlace',
          },
          {
            headerName:'關鍵零組件',
            field: 'CriticalPart',
          },
          {
            headerName:'檢驗時間(天)',
            field: 'InspectionTime',
          },
          {
            headerName:'其他內文',
            field: 'OtherDesc',
          },
          {
            headerName:'交貨日異動說明',
            field: 'ChangeDateReason',
          },
        ],
        defaultColDef : {
          editable: false,
          enableRowGroup: true,
          enablePivot: true,
          enableValue: true,
          sortable: true,
          resizable: true,
          filter: true,
          flex: 1,
          minWidth: 150,
        },
      },

      getDetailRowData: function (params) {
        params.successCallback(params.data.SrmPoLs);

      },
    };
  }

  ngOnInit(): void {
    this.isvendor=this._storageService.vendorId.length>0;
    var Werks= this._storageService.werks.split(',');
    var werkselected='';
    if(Werks.length==1)
    {
      werkselected=Werks[0];
    }
    this.searchForm = this._formBuilder.group({
      PO_NUM: this.route.snapshot.paramMap.get('number'),
      STATUS: [1],
      EkgryDesc:[null],
      DATASTATUS:[0],
      ORG:[werkselected]
    });
  }
  start(e){
    console.info(e.rowData);
    const data={
      functionId:7,
      number:e.rowData.PoNum.toString()+'-'+ e.rowData.PoLId.toString(),
      werks:e.rowData.Org,
      type:2,
      deadline:e.rowData.ReplyDeliveryDate,
      isUplaod:true,
    }
    this.filemodal.upload(data);
    //this.isVisible=true;
  }
downLoadFile(fileName)
{
  // var ip =
  // (req.headers["x-forwarded-for"] || "").split(",").pop() ||
  // req.connection.remoteAddress ||
  // req.socket.remoteAddress ||
  // req.connection.socket.remoteAddress;
  //console.info(req);
  //alert(request.connection.remoteAddress.replace(/^.*:/, ""));
this._srmPoService.DownloadFileUrl(fileName).subscribe((result: any) => {
  this._srmPoService.DownloadFilePath(result.Path,result.Name,this.polData.PoId,this.polData.PoLId).subscribe((data: any) => {
  const a = document.createElement('a');
  var re = /(?:\.([^.]+))?$/;
  var ext = re.exec(result.Name)[1];
  const mime = require('mime');
  var type=mime.getType(ext);
  const blob = new Blob([data], { 'type': type });

  //if (window.navigator && window.navigator.msSaveOrOpenBlob) {//IE
    //window.navigator.msSaveOrOpenBlob(data, result.Name);
//} else {
    // var fileURL = URL.createObjectURL(blob);
    // if (navigator.userAgent.indexOf("Chrome") != -1 || navigator.userAgent.indexOf("Firefox") != -1){
    //     let win = window.open(fileURL, '_blank');
    // } else { //Safari & Opera iOS
    //     window.location.href = fileURL;
    // }
    var url = window.URL.createObjectURL(blob);
    var anchor = document.createElement('a');
    anchor.href = url;
    anchor.target = '_blank';
    anchor.click();
  //  a.href = URL.createObjectURL(blob);
  //  a.download = result.Name;
  //  a.click();
});
});
}

  openFileModal(e){
    this.fileNameList=[];
    this.polData=e.data;
    console.info(e.data);
    //this.poData=e.data;
    var query = {
      I_EBELN: e.data.PoNum,
      I_EBELP:e.data.PoLId,
      I_MATNR: "",
      I_WERKS: e.data.Org,
    }

    //const formData = new FormData();
    //formData.append("number",e.data.PoNum);
    this._srmPoService.GetPoDoc(query).subscribe((result: any) => {

      //result[0].showDownload=true;
      console.info(result);
      this.fileNameList=result;
      // if(result.length>0)
      // {
      //     result.forEach(doc => {
      //     this.fileNameList.push(doc.doknr);
      //   });
      // }

    });
    //this.formData=formData;
    this.isVisible=true;
  }
  // openFileModal(e){
  //   console.info(e);
  //   this.poData=e.data;
  //   const formData = new FormData();
  //   formData.append("number",e.data.PoNum);
  //   this._fileService.get(1, 200, '/PoFiles/'+e.data.PoNum).subscribe((result: any) => {
  //     if(result.length>0)
  //   {
  //     var fileList: NzUploadFile[] = [
  //       {
  //         uid: e.data.PoNum,
  //         name: result[0].name,
  //         status: 'done',
  //       },
  //     ]
  //     this.fileList = fileList;
  //   }
  //   });
  //   this.formData=formData;
  //   console.info(this.formData);
  //   this.isVisible=true;
  // }
  expiryDateFormatter(params) {
    if (params.value) {
      return `${params.value.date.month - 1}/${params.value.date.year}`;
     }
  }
  onFirstDataRendered(params) {
    // setTimeout(function () {
    //   params.api.getDisplayedRowAtIndex(1).setExpanded(true);
    // }, 0);
  }

  onGridReady(params) {
    this.gridApi = params.api;
    console.info(this.gridApi);
    this.gridColumnApi = params.columnApi;
    console.info(this.gridColumnApi);
    this.refresh();
    //this.getPoList(null);
    // this._srmPoService.GetPo(null)
    //   .subscribe((data) => {
    //     this.rowData = data;
    //   });
  }
  submitSearch() {
    this.refresh();
  }
  pageChange() {
    this.refresh();
  }
  handleOk(): void {
    this.modalclose();
    var query = {
      I_EBELN: this.polData.PoNum,
      I_EBELP: this.polData.PoLId,
      I_MATNR: this.polData.MatnrId,
      DESCRIPTION: this.polData.Description,
      VENDOR_ID: this.polData.VendorId,
      fileNameList: this.fileNameList,
    }
    this._srmPoService.UpdatePoLDoc(query).subscribe((result: any) => {
      console.info(result);
    });
    //呼叫後台保存filename跟active跟ponum,polid
  }

  handleCancel(): void {
    console.info(this.poData);
    this.modalclose();
  }
  download= (file: NzUploadFile): void => {
    console.info(file);
     this._srmFileService.downloadPoFile(file.uid,file.name).subscribe((result: any) => {
       const a = document.createElement('a');
       const blob = new Blob([result], { 'type': "application/octet-stream" });
       a.href = URL.createObjectURL(blob);
       a.download = file.name;
       a.click();
     });
  }
  beforeUpload = (singleFile: File, fileList: File[]): boolean => {
    this.fileList = fileList;
    this.formData.append('files',singleFile);
    console.log(this.fileList);
    return false;
  };
  handleUpload() {
    this.uploading = true;
    this._srmFileService.UploadPoFile(this.formData).subscribe(result => {
          this.modalclose();
          this._messageService.success("上載完畢！");
          window.location.reload();
        }, error => {
          this.uploading = false;
        });
  }
  modalclose()
  {
    this.fileList=[];
    this.formData=new FormData;
    this.uploading=false;
    this.isVisible=false;
  }

  // download= (file: NzUploadFile): void => {
  //   console.info(file);
  //    this._srmFileService.download(file.uid,file.name).subscribe((result: any) => {
  //      const a = document.createElement('a');
  //      const blob = new Blob([result], { 'type': "application/octet-stream" });
  //      a.href = URL.createObjectURL(blob);
  //      a.download = file.name;
  //      a.click();
  //    });
  // }

  delete=(file: NzUploadFile):void=> {
    this._modalService.confirm({
      nzTitle: '是否刪除?',
      nzContent: null,
      nzOnOk: () => {
          this._srmFileService.delete(file.uid).subscribe(result => {
            this._messageService.success("刪除成功！");
            this.fileList=[];
            //this.refresh();
          });

      }
    });
  }
  refresh() {
    var query = {
      poId:this.searchId,
      poNum: this.searchForm.value["PO_NUM"] == null ? "" : this.searchForm.value["PO_NUM"],
      status: this.searchForm.value["STATUS"] == null ? "0" : this.searchForm.value["STATUS"],
      ekgryDesc: this.searchForm.value["EkgryDesc"] == null ? "" : this.searchForm.value["EkgryDesc"],
      dataStatus:this.searchForm.value["DATASTATUS"] == null ? "0" : this.searchForm.value["DATASTATUS"],
      org: this.searchForm.value["ORG"] == null ? "" : this.searchForm.value["ORG"],
      page: this.page,
      size: this.size
    }
    this.getPoList(query);
  }
  getPoList(query){
    if(query==null)
    {
      query = {
        poNum: "",
        status: "0",
        ekgryDesc: "",
        dataStatus:"0",
        page: this.page,
        size: this.size,
        org:""
      }
    }
    this._srmPoService.GetPo(query)
      .subscribe((result) => {
        this.rowData = result;
      });
  }
  getContextMenuItems(params) {
    var result = [
      {
        name: '新增主題回復',
        action: function () {
          window.location.href="/srm/diss-add";
          //window.alert('Alerting about ' + params.value);
        },
        cssClasses: ['redFont', 'bold'],
      },
    ];
    return result;
  }
  submitEdit(){}
}
function dateFormatter(data) {
  if(data.value==null) return "";
  var date=new Date(data.value);
  return `${formatDate(date, 'yyyy-MM-dd', 'zh-Hant-TW', '+0800')}`;
  return `${date.getDate()}`;
  return `${date.getFullYear()}-${date.getMonth()+1}-${date.getDate()}`;
}
function getDatePicker() {
  function Datepicker() {}
  Datepicker.prototype.init = function (params) {
    this.eInput = document.createElement('input');
    this.eInput.value = params.value;
    this.eInput.classList.add('ag-input');
    this.eInput.style.height = '100%';
    $(this.eInput).datepicker({ dateFormat: 'dd/mm/yy' });
  };
  Datepicker.prototype.getGui = function () {
    return this.eInput;
  };
  Datepicker.prototype.afterGuiAttached = function () {
    this.eInput.focus();
    this.eInput.select();
  };
  Datepicker.prototype.getValue = function () {
    return this.eInput.value;
  };
  Datepicker.prototype.destroy = function () {};
  Datepicker.prototype.isPopup = function () {
    return false;
  };
  return Datepicker;
}
