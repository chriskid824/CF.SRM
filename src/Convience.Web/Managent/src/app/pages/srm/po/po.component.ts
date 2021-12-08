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
  uploading: boolean = false;
  formData:FormData;
  poData:any;
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
    private dialog: MatDialog,private route: ActivatedRoute) {
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
        headerName:'採購單號',
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
        headerName:'採購單總金額',
        field: 'TotalAmount',
      },
      {
        headerName:'採購人員',
        field: 'Buyer',
        editable:true
      },
      {
        headerName:'採購組織',
        field: 'Org',
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
        headerName:'拋轉日期',
        field: 'CreateDate',
        valueFormatter:dateFormatter
      },
      { headerName: '操作', field: 'fieldName',
      cellRenderer : function(params){
        if(params.data.Status==21)
        {
          var eDiv = document.createElement('div');
          if(params.data.hasFile)
          {
            eDiv.innerHTML = '<span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple" style="height:39px">接收</button></span><span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple2" style="height:39px">檔案</button></span>';
            var eButton = eDiv.querySelectorAll('.btn-simple')[0];
            eButton.addEventListener('click', function() {
              _srmPoService.UpdateStatus(params.data.PoId).subscribe(result=>{
                alert('採購單號:'+params.data.PoNum+'已接收');
                params.data.Status=11;
                params.data.ReplyDate=new Date();
                params.data.StatusDesc="待回覆";
                params.data.SrmPoLs.forEach(element => {
                  element.Status=11;
                  element.StatusDesc="待回覆";
                  eDiv.innerHTML='';
                });
                params.api.refreshCells();
              });
            });
          }
          else
          {
            eDiv.innerHTML = '<span class="my-css-class"><button *canOperate="\'PO_ACCEPT\'" nz-button nzType="primary" class="btn-simple2" style="height:39px">檔案</button></span>';
          }

          var eButton_file = eDiv.querySelectorAll('.btn-simple2')[0];

          eButton_file.addEventListener('click', function() {
            params.ondblclick(params);
          });
          return eDiv;
          }
        },
        cellRendererParams: {
          ondblclick:this.openFileModal.bind(this),
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
      minWidth: 100,
    };
    this.components = { datePicker: getDatePicker()};
    this.detailCellRendererParams = {
      detailGridOptions: {
        frameworkComponents:this.frameworkComponents,
        columnDefs: [
          {
            headerName:'採購單明細識別碼',
            field: 'PoLId',
            hide:'true',
          },
          {
            headerName:'採購單號',
            field: 'PoNum',
            hide:'true',
          },
          {
            headerName:'料號識別碼',
            field: 'Matnr',
          },
          {
            headerName:'物料內文',
            field: 'Description',
          },
          {
            headerName:'數量',
            field: 'Qty',
          },
          {
            headerName:'單價',
            field: 'Price',
          },
          {
            headerName:'交貨日期',
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
              'rag-lime': 'x == null',
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
            headerName:'關鍵零組件/首件',
            field: 'CriticalPart',
          },
          {
            headerName:'檢驗時間(天)',
            field: 'InspectionTime',
          },
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
             Width:20,
             cellRenderer: 'buttonRenderer',
             cellRendererParams: {
              ondblclick:this.start.bind(this),
              label: '',
            },
            pinned: 'left',
          }
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
          minWidth: 100,
        },
      },

      getDetailRowData: function (params) {
        console.info(params);
        params.successCallback(params.data.SrmPoLs);

      },
    };
  }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      PO_NUM: this.route.snapshot.paramMap.get('number'),
      STATUS: [1],
      BUYER:[null],
      DATASTATUS:[0]
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
  openFileModal(e){
    console.info(e);
    this.poData=e.data;
    const formData = new FormData();
    formData.append("number",e.data.PoNum);
    this._fileService.get(1, 200, '/PoFiles/'+e.data.PoNum).subscribe((result: any) => {

      //result[0].showDownload=true;
      console.info(result);
      if(result.length>0)
    {
      var fileList: NzUploadFile[] = [
        {
          uid: e.data.PoNum,
          name: result[0].name,
          status: 'done',
          //response: 'Server Error 500', // custom error message to show
          //url: '/PoFiles/'+e.data.PoNum
        },
      ]
      //var file=[{name:result[0].name}];
      // var file:File=new File(){};
      // file.name=result.fileName;
      // file.showDownload=true;
      //console.info(file);
      this.fileList = fileList;
    }

    });
    this.formData=formData;
    console.info(this.formData);
    this.isVisible=true;
  }
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
    this.gridColumnApi = params.columnApi;
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
      buyer: this.searchForm.value["BUYER"] == null ? "" : this.searchForm.value["BUYER"],
      dataStatus:this.searchForm.value["DATASTATUS"] == null ? "0" : this.searchForm.value["DATASTATUS"],
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
        buyer: "",
        dataStatus:"0",
        page: this.page,
        size: this.size
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
