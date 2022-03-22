import { Component, OnInit,ViewEncapsulation,ViewChild,TemplateRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NzDatePickerComponent} from 'ng-zorro-antd/date-picker';
import datepickerFactory from 'jquery-datepicker';
import datepickerJAFactory from 'jquery-datepicker/i18n/jquery.ui.datepicker-en-GB';
import { AgGridDatePickerComponent} from '../po/AGGridDatePickerCompponent';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { SrmDeliveryService } from '../../../business/srm/srm-delivery.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";
import { DeliveryModalComponent } from "../delivery-modal/delivery-modal.component";
import { DeliveryAddComponent } from "../delivery-add/delivery-add.component";
import { SrmModule } from '../srm.module';
import { EditButtonComponent } from './button-cell-renderer.component';
import { ActivatedRoute } from '@angular/router';
class DialogData {
  paramid: string;
  paramname:string;
  columnname:string;
  valuetype:string;
  data: any;
  constructor() {  }
}
@Component({
  selector: 'app-delyvery-l',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './delyvery-l.component.html',
  styleUrls: ['./delyvery-l.component.less']
})
export class DelyveryLComponent implements OnInit {
  gridApi;
  gridColumnApi;
  columnDefs;
  defaultColDef;
  detailCellRendererParams;
  components;
  rowData: any;
  searchForm: FormGroup = new FormGroup({});
  page: number = 1;
  size: number = 2;
  total: number;
  tplModal: NzModalRef;
  public isVisible = false;
  editForm: FormGroup = new FormGroup({});
  frameworkComponents: any;
  isedit:boolean;
  PoNumOption = [];
  PoNumOptionExceptCurrent = [];
  PoLIdOption=[];
  PoList;
  currenPoLID;
  MaxDeliveryQty:number=0;
  @ViewChild('ctest1')
  ctest1: TemplateRef<any>;

  @ViewChild('ctest2')
  ctest2: TemplateRef<any>;
  constructor(private _formBuilder: FormBuilder,private http: HttpClient,private _srmPoService: SrmPoService,private _srmDeliveryService: SrmDeliveryService
    ,private _modalService: NzModalService, private dialog: MatDialog,private route: ActivatedRoute) {
      this.isedit=true;
      this.frameworkComponents = {
        buttonRenderer: EditButtonComponent,
      }
    this.columnDefs = [
      {
        headerName:'交貨單號',
        field: 'DeliveryNum',
        cellRenderer: 'agGroupCellRenderer',
      },
      {
        headerName:'交貨單識別碼',
        field: 'DeliveryId',
        hide:true
      },
      {
        headerName:'狀態',
        field: 'Status',
        cellClassRules: {
          'rag-green': 'x == 16',
          'rag-red': 'x == 15',
        },
        valueFormatter:'switch(value){case 12 : return "已收貨";case 15 : return "待交貨"; case 14 : return "待收貨"; default : return value;}'
      },
      {
        headerName:'本次需求日',
        field: 'CreateDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'建立日期',
        field: 'CreateDate',
        valueFormatter:dateFormatter
      },
      {
        headerName:'建立人員',
        field: 'CreateBy',
      },
       { headerName: '操作', field: 'fieldName',
      //  innerWidth:500,
      //  cellRenderer: 'buttonRenderer',
      //   cellRendererParams: {
      //     onClick: this.add.bind(this),
      //     oncancel:this.cancel.bind(this),
      //     ondblclick:this.start.bind(this),
      //     onstorage:this.save.bind(this),
      //     label: '',
      //   },
        pinned: 'left',
       cellRenderer : function(params){
         if(params.data.Status==14)
         {
           var eDiv = document.createElement('div');
           //eDiv.innerHTML = '<span class="my-css-class" style="width:100%"><button nz-button nzType="primary" class="btn-simple" style="height:39px"><i nz-icon nzType="delete"></i>列印出貨單</button><button nz-button nzType="primary" class="btn-edit" style="height:39px;margin-left:10px;"><i nz-icon nzType="delete"></i>編輯</button><button nz-button nzType="primary" class="btn-save" style="height:39px;margin-left:10px;"><i nz-icon nzType="save"></i>保存</button><button nz-button nzType="primary" class="btn-cancel" style="height:39px;margin-left:10px;"><i nz-icon nzType="cancel"></i>取消</button><button nz-button nzType="primary" class="btn-add" style="height:39px;margin-left:10px;"><i nz-icon nzType="add"></i>新增</button></span>';
           eDiv.innerHTML = '<span class="my-css-class"><button nz-button nzType="primary" class="btn-simple" style="height:39px">列印出貨單</button></span>';
           var eButton = eDiv.querySelectorAll('.btn-simple')[0];
          //  var eButton_edit = eDiv.querySelectorAll('.btn-edit')[0];
          //  var eButton_add = eDiv.querySelectorAll('.btn-add')[0];
          //  var eButton_save = eDiv.querySelectorAll('.btn-save')[0];
          //  var eButton_cancel = eDiv.querySelectorAll('.btn-cancel')[0];
          //  eButton_save.setAttribute('hidden','true');
          //  eButton_cancel.setAttribute('hidden','true');
           //eButton.addEventListener('click', showDialog);
            eButton.addEventListener('click', function() {
              var dialogData=new DialogData();
              dialogData.data=params.data;
              const dialogConfig = new MatDialogConfig();
              dialogConfig.disableClose = true;
              dialogConfig.autoFocus = true;
              //dialogConfig.minWidth = "1500px";
              dialogConfig.maxHeight = "1500px";
              dialogConfig.data = dialogData;
              dialog.open(DeliveryModalComponent, dialogConfig);
            });
            // eButton_edit.addEventListener('click', function() {
            //   // params.api.forEachDetailGridInfo(function(detailGridInfo) {
            //   //   console.info(detailGridInfo);
            //   // });
            //   // var detailGridID = eButton_edit.closest('div[row-id]')
            //   // params.api.setRowNodeExpanded({rowNode:params.node,expanded:true});
            //   // params.node.expanded = true;
            //   // params.api.onGroupExpandedOrCollapsed();
            //   eButton_edit.setAttribute('hidden','true');
            //   eButton_save.removeAttribute('hidden');
            //   eButton_cancel.removeAttribute('hidden');
            //   var detailGrid = params.api.getDetailGridInfo("detail_"+params.rowIndex);
            //   if (detailGrid) {
            //     var length=params.data.SrmDeliveryLs.length;
            //     detailGrid.api.setFocusedCell(0, 'DeliveryQty');
            //     for(var i=0;i<length;i++)
            //     {
            //       detailGrid.api.startEditingCell({
            //         rowIndex: i,
            //         colKey: 'DeliveryQty',
            //       });
            //     }
            //   }


            //   //  console.info(params);
            //   // params.api.setFocusedCell(2, 'DeliveryNum');

            // });
            // eButton_add.addEventListener('click', function() {
            //   // params.node.expanded = true;
            //   // params.api.onGroupExpandedOrCollapsed();
            //   var dialogData=new DialogData();
            //   dialogData.data=params.data;
            //   const dialogConfig = new MatDialogConfig();
            //   dialogConfig.disableClose = true;
            //   dialogConfig.autoFocus = true;
            //   dialogConfig.minWidth = "250px";
            //   //dialogConfig.maxHeight = "250px";
            //   dialogConfig.data = dialogData;
            //   const dialogRef =dialog.open(DeliveryAddComponent, dialogConfig);
            //   dialogRef.afterClosed().subscribe(result => {
            //     console.log('The dialog was closed');
            //     if(result!=undefined)
            //     {

            //     }

            //   });
            // });
            // eButton_save.addEventListener('click', function() {
            //   eButton_edit.removeAttribute('hidden');
            //   eButton_save.setAttribute('hidden','true');
            //   eButton_cancel.setAttribute('hidden','true');
            //   var detailGrid = params.api.getDetailGridInfo("detail_"+params.rowIndex);
            //   if (detailGrid) {
            //     var length=params.data.SrmDeliveryLs.length;
            //     for(var i=0;i<length;i++)
            //     {
            //       detailGrid.api.stopEditing({
            //         rowIndex: i,
            //         colKey: 'DeliveryQty',
            //       });
            //     }
            //   }
            // });
            // eButton_cancel.addEventListener('click', function() {
            //   eButton_edit.removeAttribute('hidden');
            //   eButton_save.setAttribute('hidden','true');
            //   eButton_cancel.setAttribute('hidden','true');
            //   var detailGrid = params.api.getDetailGridInfo("detail_"+params.rowIndex);
            //   if (detailGrid) {
            //     var length=params.data.SrmDeliveryLs.length;
            //     for(var i=0;i<length;i++)
            //     {
            //       detailGrid.api.stopEditing({
            //         rowIndex: i,
            //         colKey: 'DeliveryQty',
            //       });
            //     }
            //   }
            // });
              //this.showDialog();
               //  this.tplModal = _modalService.create({
              //     nzTitle: "123",
              //     nzContent: `123`,
               //    nzFooter: null,
              //   });
              // this._roleService.getRole(role.id).subscribe(reuslt => {
              //   this.editedRole = reuslt;
              //   this.editForm = this._formBuilder.group({
              //     roleName: [this.editedRole.name, [Validators.required, Validators.maxLength(15)]],
              //     remark: [this.editedRole.remark, [Validators.maxLength(30)]],
              //     menus: [this.editedRole.menus?.split(',')]
              //   });
              //   this.tplModal = this._modalService.create({
              //     nzTitle: 123,
              //     nzContent: 123,
              //     nzFooter: null,
              //   });

              // });
            //   _srmPoService.UpdateStatus(params.data.PoId).subscribe(result=>{
            //     alert('採購單號:'+params.data.DeliveryNum+'已交貨');
            //     params.data.Status=11;
            //     params.data.ReplyDate=new Date();
            //    params.api.refreshCells();
            //  });
            //});
           return eDiv;
           }
         }

    },

    //    {
    //     headerName: "",
    //     width: 100,
    //     cellRenderer: (data) => {
    //       return `<i nz-icon nzType="double-left" nzTheme="outline"></i>`;
    //   }
    // }
    ];

    this.defaultColDef = { flex: 1,resizable: true, };
    this.components = { datePicker: getDatePicker()};
    this.detailCellRendererParams = {
      detailGridOptions: {
        frameworkComponents:this.frameworkComponents,
        columnDefs: [
          {
            headerName:'項次',
            field: 'DeliveryLId',
            hide:true,
          },
          {
            headerName:'項次',
            valueGetter: "node.rowIndex + 1"
          },
          {
            headerName:'物料',
            field: 'Matnr',
          },
          {
            headerName:'採購單碼',
            field: 'PoNum',
          },
          {
            headerName:'採購項次',
            field: 'PoLId',
          },
          {
            headerName:'交貨數量',
            field: 'DeliveryQty',
            editable:false,
          },
          {
            headerName:'已品檢數量',
            field: 'QmQty',
          },
          { headerName: '操作',
            Width:120,
            cellRenderer: 'buttonRenderer',
            cellRendererParams: {
             onClick: this.add.bind(this),
             oncancel:this.cancel.bind(this),
             ondblclick:this.start.bind(this),
             onstorage:this.save.bind(this),
             label: '',
           },
           pinned: 'left',
          }
        ],
        defaultColDef: { flex: 1 },
        stopEditingWhenCellsLoseFocus: false,
      },

      getDetailRowData: function (params) {
        params.successCallback(params.data.SrmDeliveryLs);

      },
      getRowNodeId: function (data) {
        // use 'account' as the row ID
        return data.DeliveryNum;
      },
    };
  }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      DELIVERY_NUM: this.route.snapshot.paramMap.get('number'),
      STATUS: [1],
    });
    //this.refresh();
    this.getPoLList(null);
  }
  getPoLList(query){
    if(query==null)
    {
      query = {
        poNum: "",
        status: "15",
        replyDeliveryDate_s: null,
        replyDeliveryDate_e: null,
      }
    }
    this._srmPoService.GetPoL(query)
      .subscribe((result) => {
        this.PoList=result;
        var newoptions=[];
        for (var po in result) {
            if(!newoptions.some(p=>p.value==result[po].PoId))
            {
              newoptions.push({label:result[po].PoNum,value:result[po].PoId});
            }
          }
          this.PoNumOption=newoptions;
          this.PoNumOptionExceptCurrent=this.PoNumOption;
        //this.rowData = result;
      });
  }
  onChange(value) {
    var poLIdList=this.PoList.filter(p=>p.PoId==value);
    var newoptions2=[];
    for(var poL in poLIdList)
    {
      newoptions2.push({label:poLIdList[poL].PoLId.toString(),value:poLIdList[poL].PoLId.toString()});
    }
    this.PoLIdOption=newoptions2;
  }
  onPoLIdChange() {
    console.info(this.editForm.get('PoNum').value);
    var PoId=this.editForm.get('PoNum').value;
    var PoLId=this.editForm.get('PoLId').value;
    if(PoId==null||PoLId==null||this.currenPoLID==PoLId)
    { return; }
    this.currenPoLID=PoLId;
    var PoLItem= this.PoList.find(p=>p.PoId==PoId &&p.PoLId==PoLId);
    this.MaxDeliveryQty=PoLItem.DeliveryQty;
    console.info(PoLItem);
    this.editForm.setValue({
      DeliveryId: this.editForm.get('DeliveryId').value,
      DeliveryLId: null
      , Matnr: PoLItem.Matnr
      , PoNum: PoLItem.PoId
      , PoLId: PoLItem.PoLId
      , DeliveryQty: PoLItem.DeliveryQty
    });
    // var query = {
    //   poNum: PoNum,
    //   poLId: PoLId,
    // }
    // this._srmPoService.GetPoL(query)
    //   .subscribe((result) => {
    //     console.info(result);
    //   });
  }
  expandSet = new Set<string>();
  onExpandChange(id: string, checked: boolean): void {
    if (checked) {
      this.expandSet.add(id);
    } else {
      this.expandSet.delete(id);
    }
  }

  showprint(data)
  {
    var dialogData=new DialogData();
    dialogData.data=data;
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.minWidth = "1500px";
    dialogConfig.maxHeight = "1500px";
    dialogConfig.data = dialogData;
    this.dialog.open(DeliveryModalComponent, dialogConfig);
  }
  editCache: { [key: string]: { edit: boolean; data: any } } = {};
  // startEdit(id: string): void {
  //   console.info(this.editCache);
  //   this.editCache[id].edit = true;
  // }

  // cancelEdit(id: string): void {
  //   const index = this.rowData.findIndex(item => item.DeliveryNum === id);

  //   this.editCache[id] = {
  //     data: { ...this.rowData[index] },
  //     edit: false
  //   };
  // }

  // saveEdit(id: string): void {
  //   const index = this.rowData.findIndex(item => item.DeliveryNum === id);
  //   Object.assign(this.rowData[index], this.editCache[id].data);
  //   this.editCache[id].edit = false;
  // }

  updateEditCache(): void {
    this.rowData.forEach(item => {
      this.editCache[item.DeliveryNum] = {
        edit: false,
        data: { ...item }
      };
    });
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
    this.gridApi.sizeColumnsToFit();
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
  refresh() {
    var query = {
      deliveryNum: this.searchForm.value["DELIVERY_NUM"] == null ? "" : this.searchForm.value["DELIVERY_NUM"],
      status: this.searchForm.value["STATUS"] == null ? "0" : this.searchForm.value["STATUS"],
    }
    this.getDelyveryList(query);
  }
  getDelyveryList(query){
    if(query==null)
    {
      query = {
        deliveryNum: "",
        status: "0",
      }
    }
    this._srmDeliveryService.GetDelivery(query)
      .subscribe((result) => {
        this.rowData = result;
        if(Object.values(this.editCache).filter((item) => item != null).length==0)
        {
          this.updateEditCache();
        }
      });
  }


  add(e) {
    var ids=this.rowData.filter(p=>p.DeliveryId==e.rowData.DeliveryId)[0].SrmDeliveryLs.map(({ PoNum }) => PoNum);
    this.PoNumOptionExceptCurrent=this.PoNumOption.filter(p=>!ids.includes(p.label));
    this.isedit=false;
    this.editForm = this._formBuilder.group({
      DeliveryId: [{value:null,disabled: true}, [Validators.required]],
      DeliveryLId: [{value:null,disabled: true}, [Validators.required]],
      //price: ['', [Validators.pattern(/^(0|([1-9](\d)*))(\.(\d)*)?$/)]],
      Matnr: [{value:null,disabled: true}, [Validators.required, Validators.pattern(SrmModule.decimalTwoDigits)]],
      PoNum: [{value:null,disabled: this.isedit}, [Validators.required, Validators.pattern(SrmModule.number)]],
      PoLId: [{value:null,disabled: this.isedit}, [Validators.required]],
      DeliveryQty: [{value:null,disabled: this.isedit}, [Validators.required]],
    });
    this.editForm.setValue({
      DeliveryId: e.rowData.DeliveryId,
      DeliveryLId: null
      , Matnr: null
      , PoNum: null
      , PoLId: null
      , DeliveryQty: null
    });

    this.tplModal = this._modalService.create({
      nzTitle: this.ctest1,
      nzContent: this.ctest2,
    nzFooter: null,
    nzClosable: true,
    nzMaskClosable: false,
  });
  }
  cancel(e) {
    this._modalService.confirm({
      nzTitle: '你確定要刪除項次'+e.rowData.DeliveryLId+'?',
      //nzContent: '<b style="color: red;">Some descriptions</b>',
      nzOkText: '確認',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => this.submitDelete(e.rowData),
      nzCancelText: '取消',
      //nzOnCancel: () => alert("取消")
    });

  }
  submitDelete(data){
    this._srmDeliveryService.DeleteDeliveryL(data)
    .subscribe((result) => {
      if(result==null)
      {
       var selectedRows = this.gridApi.getRenderedNodes();
       this.gridApi.setRowData(this.rowData);

       this.gridApi.forEachLeafNode((node) => {
         if (selectedRows.find(s => s.DeliveryId == node.data.DeliveryId)){
           node.setRowNodeExpanded(true);
         }
       });
       alert("修改成功");
       this.refresh();
       this.tplModal.close();
      }
    });
  }
  start(e){

    var PoLItem= this.PoList.filter(p=>p.PoId==e.rowData.PoId &&p.PoLId==e.rowData.PoLId);
    console.info(this.PoList);
    console.info(PoLItem);
    console.info(e.rowData);
    this.MaxDeliveryQty=PoLItem.DeliveryQty+e.rowData.DeliveryQty;
    this.isedit=true;
    console.info(e.rowData);
    console.info(this.rowData);
    this.editForm = this._formBuilder.group({
      DeliveryId: [{value:null,disabled: true}, [Validators.required]],
      DeliveryLId: [{value:null,disabled: true}, [Validators.required]],
      //price: ['', [Validators.pattern(/^(0|([1-9](\d)*))(\.(\d)*)?$/)]],
      Matnr: [{value:null,disabled: true}, [Validators.required, Validators.pattern(SrmModule.decimalTwoDigits)]],
      PoNum: [{value:null,disabled: true}, [Validators.required, Validators.pattern(SrmModule.number)]],
      PoLId: [{value:null,disabled: true}, [Validators.required]],
      DeliveryQty: [{value:null,disabled: false}, [Validators.required]],
    });
    this.editForm.setValue({
      DeliveryId: e.rowData.DeliveryId,
      DeliveryLId: e.rowData.DeliveryLId
      , Matnr: e.rowData.Matnr
      , PoNum: e.rowData.PoNum
      , PoLId: e.rowData.PoLId
      , DeliveryQty: e.rowData.DeliveryQty
    });
    this.tplModal = this._modalService.create({
      nzTitle: this.ctest1,
      nzContent: this.ctest2,
    nzFooter: null,
    nzClosable: true,
    nzMaskClosable: false,
  });
  }
  save(e){
  }
  edit() {
    //console.log(this.CurrencyList.find(r => r.currency == this.currency.value)?.currencyName);
    //console.log(this.tempqotId);
    console.log(this.editForm);

     for (const i in this.editForm.controls) {
       this.editForm.controls[i].markAsDirty();
       this.editForm.controls[i].updateValueAndValidity();
     }
     if (this.editForm.valid) {
       if(this.editForm.get('DeliveryQty').value>this.MaxDeliveryQty)
       {
        alert("最大數量為"+this.MaxDeliveryQty);
        return;
       }
       if(this.isedit)
       {
        var r = this.rowData.find(r => r.DeliveryId == this.editForm.get('DeliveryId').value).SrmDeliveryLs.find(p=>p.PoLId==this.editForm.get('PoLId').value);

        r.DeliveryQty = this.editForm.get('DeliveryQty').value;
     //   r.standQty = this.editForm.get('standQty').value;
     //   r.minQty = this.editForm.get('minQty').value;
     //   r.taxcode = this.editForm.get('taxcode').value;//.editForm.get('ekgry').value;
     //   r.taxcodeName = this.TaxcodeList.find(r => r.taxcode == this.editForm.get('taxcode').value)?.taxcodeName;
     //   r.currency = this.editForm.get('currency').value;
     //   r.currencyName = this.CurrencyList.find(r => r.currency == this.editForm.get('currency').value)?.currencyName;
     //   r.effectiveDate = dateFormatter(this.editForm.get('effectiveDate').value);
     //   r.expirationDate = dateFormatter(this.editForm.get('expirationDate').value);
     //   r.note = this.editForm.get('note').value;
     this._srmDeliveryService.UpdateDeliveryL(r)
     .subscribe((result) => {
       if(result==null)
       {
        // var selectedRows = this.gridApi.getRenderedNodes();

        // this.rowData.splice(selectedRows.rowIndex, 1);
        // this.gridApi.setRowData(this.rowData);
        // this.gridApi.forEachLeafNode((node) => {
        //   if (selectedRows.find(s => s.DeliveryId == node.data.DeliveryId)){
        //     node.setRowNodeExpanded(true);
        //   }
        // });
        alert("修改成功");
        this.refresh();
       }
     });

       }
else
{
   var DeliveryL={
     DeliveryId:this.editForm.get('DeliveryId').value,
     PoId:this.editForm.get('PoNum').value,
     PoLId:this.editForm.get('PoLId').value,
     DeliveryQty:this.editForm.get('DeliveryQty').value,
     QmQty:0,
   };
   this._srmDeliveryService.UpdateDeliveryL(DeliveryL)
   .subscribe((result) => {
     if(result==null) alert('操作成功');
     this.refresh();
   });
}
     }
  }

  cancelEdit() {
    this.tplModal.close();
  }
}
function dateFormatter(data) {
  if(data.value==null) return "";
  var date=new Date(data.value);
  return `${date.getFullYear()}-${date.getMonth()+1}-${date.getDate()}`;
}
function getDatePicker() {
  function Datepicker() {}
  // Datepicker.prototype.init = function (params) {
  //   this.eInput = document.createElement('input');
  //   this.eInput.value = params.value;
  //   this.eInput.classList.add('ag-input');
  //   this.eInput.style.height = '100%';
  //   $(this.eInput).datepicker({ dateFormat: 'dd/mm/yy' });
  // };
  // Datepicker.prototype.getGui = function () {
  //   return this.eInput;
  // };
  // Datepicker.prototype.afterGuiAttached = function () {
  //   this.eInput.focus();
  //   this.eInput.select();
  // };
  // Datepicker.prototype.getValue = function () {
  //   return this.eInput.value;
  // };
  // Datepicker.prototype.destroy = function () {};
  // Datepicker.prototype.isPopup = function () {
  //   return false;
  // };
  // return Datepicker;
}
