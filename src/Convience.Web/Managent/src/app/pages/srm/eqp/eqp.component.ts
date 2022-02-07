import { ANALYZE_FOR_ENTRY_COMPONENTS, Component, OnInit, TemplateRef } from '@angular/core';
import { GridOptions } from 'ag-grid-community';
import { Role } from 'src/app/pages/system-manage/model/role';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { RoleService } from 'src/app/business/system-manage/role.service';
import { MenuService } from 'src/app/business/system-manage/menu.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Material } from 'src/app/pages/srm/model/material';
import { SrmQotService } from '../../../business/srm/srm-qot.service';
import { Process } from '../model/Process';
import { Surface } from '../model/Surface';
import { Other } from '../model/Other';
//import { NzTreeNodeOptions } from 'ng-zorro-antd/tree';
import { Qot, QotH, QotV, reject } from '../model/Qot';
import { ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NzTreeNodeOptions } from 'ng-zorro-antd/tree';
import { runInThisContext } from 'node:vm';
import { StorageService } from '../../../services/storage.service';
import { LayoutComponent } from '../../layout/layout/layout.component';
import { SrmModule } from '../srm.module';
import { SrmEqpService } from '../../../business/srm/srm-eqp.service';
import { query } from '@angular/animations';
import { stringify } from 'node:querystring';
import { EqpH } from '../model/Eqp';
//import { data } from 'jquery';
import { FileModalComponent } from '../file-modal/file-modal.component';
import { Guid } from 'guid-typescript';
import { int } from '@zxing/library/esm/customTypings';

@Component({
  selector: 'app-eqp',
  templateUrl: './eqp.component.html',
  styleUrls: ['./eqp.component.less']
})
export class EqpComponent implements OnInit {
  @ViewChild('filemodal')
  filemodal: FileModalComponent;
  eqpinfo1: FormGroup = new FormGroup({});
  eqpinfo2: FormGroup = new FormGroup({});
  name;
  poId;
  matnr;
  no;
  matnrId;
  poQty;
  description;
  deliveryDate;
  EQP;
  poNo;
  ekgry;
  ekgryid;
  eqpId;
  eqpNum;
  createBy;
  createDate;
  woNum;
  ngQty;
  version;
  file;
  ngDesc;
  causeAnalyses;
  disposition;
  causeDept;
  qcDispoaition;
  qdrNum;
  qcNote;
  reworkCosts;
  peAction;
  status;
  canModify;
  canDelete;
  werks;
  guid: string;

  //initdate;
  constructor(
    private activatedRoute: ActivatedRoute, private _router: Router, private _layout: LayoutComponent, private _formBuilder: FormBuilder, private _storageService: StorageService, private _srmEqpService: SrmEqpService,) {
    this.name = this._storageService.Name;
    //this.delivery_date = "2021/08/08";
    this.EQP = {
    }
    this.activatedRoute.params.subscribe(params => {
      this.eqpId = params['id'];
      console.log(params['id']);
      console.log("eqpId:" + this.eqpId); // Print the parameter to the console.
    });
  }

  ngOnInit(): void {

    this.name = this._storageService.Name;
    this.eqpinfo1 = this._formBuilder.group({
      eqpNum: [{ value: null, disabled: false }],
      createBy: [{ value: null, disabled: false }],
      createDate: [{ value: null, disabled: false }],
      woNum: [{ value: null, disabled: false }],
      poId: [{ value: null, disabled: false }],
      matnr: [{ value: null, disabled: false }],
      no: [{ value: null, disabled: false }],
      ekgry: [{ value: null, disabled: false }],
      ekgryid: [{ value: null, disabled: false }],
      description: [{ value: null, disabled: false }],
      deliveryDate: [{ value: null, disabled: false }],
      poQty: [{ value: null, disabled: false }],
      ngQty: [{ value: null, disabled: false }],
      version: [{ value: null, disabled: false }],
      file: [{ value: null, disabled: false }],
      ngDesc: [{ value: null, disabled: false }],
      causeAnalyses: [{ value: null, disabled: false }],
      disposition: [{ value: null, disabled: false }],
      causeDept: [{ value: null, disabled: false }],
      qcDispoaition: [{ value: null, disabled: false }],
      qdrNum: [{ value: null, disabled: false }],
      qcNote: [{ value: null, disabled: false }],
      reworkCosts: [{ value: null, disabled: false }],
      peAction: [{ value: null, disabled: false }],
      status: [{ value: null, disabled: false }],
    });


    if (this.eqpId != undefined) {
      console.log('GetEqpData');
      this._srmEqpService.GetEqpData(this.eqpId).subscribe(result => {
        console.log(result);
        console.log(result["matnr"]);
        this.poId = result["poId"];
        this.matnr = result["matnr"];
        this.no = result["no"];
        this.matnrId = result["matnrId"];
        this.poQty = result["poQty"];
        this.description = result["description"];
        this.deliveryDate = result["deliveryDate"];
        this.poNo = result["poNo"];
        this.ekgry = result["ekgry"];
        this.ekgryid = result["ekgryid"];
        this.eqpId = result["eqpId"];
        this.eqpNum = result["eqpNum"];
        //alert(this.eqpNum)
        this.createBy = this._storageService.Name;
        this.createDate = result["createDate"];
        //alert(result["woNum"]);
        this.woNum = result["woNum"];
        this.ngQty = result["ngQty"];
        this.version = result["version"];
        this.file = result["file"];
        this.ngDesc = result["ngDesc"];
        this.causeAnalyses = result["causeAnalyses"];
        this.disposition = result["dispoaition"];
        this.causeDept = result["causeDept"];
        this.qcDispoaition = result["qcDispoaition"];
        this.qdrNum = result["qdrNum"];
        this.qcNote = result["qcNote"];
        this.reworkCosts = result["reworkCosts"];
        this.peAction = result["peAction"];
        this.canModify = result["status"] == 1;
        this.canDelete = ((result["status"] == 1) || (result["status"] == 7));
        /*因多行無法用this.canModify控制*/
        if (result["status"] != 1) {
          this.eqpinfo1 = this._formBuilder.group({
            //causeAnalyses: [{ value: null, disabled: true }]

            eqpNum: [{ value: this.eqpNum, disabled: true }],
            createBy: [{ value: this.createBy, disabled: true }],
            createDate: [{ value: this.createDate, disabled: true }],
            woNum: [{ value: this.woNum, disabled: true }],
            poId: [{ value: this.poId, disabled: true }],

            matnr: [{ value: this.matnr, disabled: true }],
            no: [{ value: this.no, disabled: true }],
            ekgry: [{ value: this.ekgry, disabled: true }],
            ekgryid: [{ value: this.ekgryid, disabled: true }],
            description: [{ value: this.description, disabled: true }],
            deliveryDate: [{ value: this.deliveryDate, disabled: true }],
            poQty: [{ value: this.poQty, disabled: true }],
            ngQty: [{ value: this.ngQty, disabled: true }],
            version: [{ value: this.version, disabled: true }],
            file: [{ value: this.file, disabled: true }],
            ngDesc: [{ value: this.ngDesc, disabled: true }],
            causeAnalyses: [{ value: this.causeAnalyses, disabled: true }],
            disposition: [{ value: this.disposition, disabled: true }],
            causeDept: [{ value: this.causeDept, disabled: true }],
            qcDispoaition: [{ value: this.qcDispoaition, disabled: true }],
            qdrNum: [{ value: this.qdrNum, disabled: true }],
            qcNote: [{ value: this.qcNote, disabled: true }],
            reworkCosts: [{ value: this.reworkCosts, disabled: true }],
            peAction: [{ value: this.peAction, disabled: true }],
            status: [{ value: this.status, disabled: true }],

          });
        }

        /* */
        this.status = result["status"];
        console.log(this.eqpinfo1);
        console.log('--ssss--');
        console.log(result["status"]);
        console.log(this.canModify)
        this.eqpinfo1.patchValue({
          eqpNum: this.eqpNum,
          createBy: this.createBy,
          createDate: this.createDate,
          woNum: this.woNum,
          poId: this.poId,
          matnr: this.matnr,
          no: this.no,
          ekgry: this.ekgry,
          ekgryid: this.ekgryid,
          description: this.description,
          deliveryDate: this.deliveryDate,
          poQty: this.poQty,
          ngQty: this.ngQty,
          version: this.version,
          file: this.file,
          ngDesc: this.ngDesc,
          causeAnalyses: this.causeAnalyses,
          disposition: this.disposition,
          causeDept: this.causeDept,
          qcDispoaition: this.qcDispoaition,
          qdrNum: this.qdrNum,
          qcNote: this.qcNote,
          reworkCosts: this.reworkCosts,
          peAction: this.peAction
        });
        //console.log(55);
      });
    } else {
      this.canModify = true;
    }
    this.name = this._storageService.Name;
    var initdate = new Date();
    this.createDate = initdate.getFullYear() + "-" + (initdate.getMonth() + 1) + "-" + initdate.getDate();
    console.log('--status--')
    console.log(this.status)
  }


  geteqp() {
    //this.eqpinfo1.value["PO_NUM"]
    var eqp = {
      h: null
    }
    var date = new Date();
    eqp.h = this.EQP;
    eqp.h.createBy = this._storageService.userName;

    console.log("------------------")
    console.log(eqp.h);


    /*this.formDetail.value["sourcer"];

    rfq.h = this.H;
    console.log('dead' + this.formDetail.get('deadline').value);*/




    eqp.h.createDate = this.createDate;
    eqp.h.woNum = this.eqpinfo1.get('woNum').value;
    eqp.h.poId = this.poId;// this.eqpinfo1.value["poId"];//this.eqpinfo1.get('poId').value;
    eqp.h.matnr = this.eqpinfo1.get('matnr').value;// (this.eqpinfo1.value["matnr"] != null) ? this.eqpinfo1.value["matnr"] : this.matnr;//this.eqpinfo1.value["matnr"];//this.matnr;
    eqp.h.no = this.eqpinfo1.get('no').value;// (this.eqpinfo1.value["no"] != null) ? this.eqpinfo1.value["no"] : this.matnr;//this.eqpinfo1.value["no"];
    eqp.h.matnrId = this.matnrId;//this.eqpinfo1.value["matnrId"];
    eqp.h.ekgryid = this.ekgryid;//this.eqpinfo1.value["ekgryid"];
    eqp.h.description = this.description;//this.eqpinfo1.value["description"];
    eqp.h.deliveryDate = this.deliveryDate; //this.eqpinfo1.value["deliveryDate"];
    eqp.h.poQty = this.poQty; //this.eqpinfo1.value["orderqty"];
    eqp.h.status = this.status;
    //alert(this.ngQty);
    //alert(this.eqpinfo1.value["ngQty"]);
    //alert(this.eqpinfo1.get("ngQty").value);
    //??ngQty version
    console.log(this.eqpinfo1);
    eqp.h.ngQty = this.eqpinfo1.value["ngQty"]; //this.eqpinfo1.value["ngQty"];//this.eqpinfo1.get("ngQty").value ;
    eqp.h.version = this.eqpinfo1.value["version"]; // this.eqpinfo1.value["version"]; //this.eqpinfo1.get("version").value ; 
    eqp.h.file = this.file;// this.eqpinfo1.value["file"]; //this.eqpinfo1.get("file").value ;
    eqp.h.ngDesc = this.eqpinfo1.value["ngDesc"];
    eqp.h.causeAnalyses = this.eqpinfo1.value["causeAnalyses"];
    eqp.h.guid = this.guid;
    eqp.h.eqpNum = this.eqpNum;
    this.activatedRoute.params.subscribe(params => {
      eqp.h.eqpId = params['id'];
    });
    //eqp.h.eqpId = this.eqpId;
    console.log("--------AA----------")
    console.log(eqp.h);


    return eqp;
  }
  //#region 刪除SRM紀錄
  delete() {
    //SRM變更狀態 -->刪除
    //將博格已起單單據作廢
    var eqp = this.geteqp();
    this._srmEqpService.Delete(eqp).subscribe(result => {
      console.log(result);
      alert('刪除成功');
      //window.close();
      //this._layout.navigateTo('eqplist');
      //this._router.navigate(['srm/eqplist']);
      window.open('../srm/eqplist', "_self");
      location.reload();
    });

  }
  //#endregion
  //#region 存檔
  save() {
    var eqp = this.geteqp();
    if (this.eqpinfo1.value["woNum"] == null) { alert("採購單未輸入"); return; }
    if ((this.eqpinfo1.value["matnr"] == null) && (this.eqpinfo1.value["no"] == null)) { alert("料號與序號請擇一輸入"); return; }
    //品名
    if ((eqp.h.description == null) || (eqp.h.description == "")) { alert("請輸入正確採購單資訊!"); return; }
    /*if (this.eqpinfo1.value["description"] == null) {
     
    }*/
    //if (isNaN(this.eqpinfo1.value["ngQty"])) {
    if (isNaN(eqp.h.ngQty)) {
      alert("異常數量格式錯誤，請輸入數值格式");
      return;
    }
    this._srmEqpService.SAVE(eqp).subscribe(result => {
      console.log(result);
      alert('保存成功');
      window.close();
      window.open('../srm/eqplist', "_self");
      location.reload();
      //this._layout.navigateTo('eqplist');
      //this._router.navigate(['srm/eqplist']);
      //location.reload();
    });
  }
  //#endregion

  //#region 起單至博格
  start() {
    var eqp = this.geteqp();
    //alert('start')
    //alert(eqp.h.woNum);
    //alert('start------')
    //alert(this.woNum)
    console.log('start------')
    console.log(eqp)
    //alert(this.eqpinfo1.get("woNum")?.value)
    //alert(this.eqpinfo1.value["ngQty"])
    //alert(eqp.h.ngQty)
    //alert(this.eqpinfo1.value["version"])
    //alert(this.eqpinfo1.value["ngDesc"])
    //alert(this.eqpinfo1.value["causeAnalyses"])

    if (this.eqpinfo1.value["woNum"] == null) { alert("採購單未輸入"); return; }
    if ((eqp.h.woNum == null) || (eqp.h.woNum = "")) { alert("採購單未輸入"); return; }

    //if (this.eqpinfo1.get("woNum").value == null) { alert("採購單未輸入"); return; }
    if ((eqp.h.matnr == null) || (eqp.h.matnr == "")) { alert("料號未輸入"); return; }
    if ((eqp.h.no == null) || (eqp.h.no == "")) { alert("序號未輸入"); return; }
    if ((eqp.h.ngQty == null) || (eqp.h.ngQty == "")) { alert("異常數量未輸入"); return; }
    if ((eqp.h.version == null) || (eqp.h.version == "")) { alert("版次未輸入"); return; }
    //alert(eqp.h.version)
    if ((eqp.h.ngDesc == null) || (eqp.h.ngDesc == "")) { alert("異常狀況及過程說明未輸入"); return; }
    if ((eqp.h.causeAnalyses == null) || (eqp.h.causeAnalyses == "")) { alert("初步肇因分析未輸入"); return; }

    /*this._srmEqpService.SAVE(eqp).subscribe(result => {
      console.log(result);
    });*/
    this._srmEqpService.Start(eqp).subscribe(result => {
      alert('起單成功');
      //window.close();
      //this._layout.navigateTo('eqplist');
      //this._router.navigate(['srm/eqplist']);
      window.open('../srm/eqplist', "_self");
      location.reload();
    });
  }
  //#endregion
  //#region 取SRM table，博格會回寫
  GetEQPData() { }
  //#endregion
  searchPo() {
    this.onRefreshPo();
    //alert(this.eqpinfo1.value["poId"])
    /*if((this.poId == null) ||(this.poId == ""))
    {
      alert('查無對應採購單資訊!')
    }*/
  }


  /*eqp.h.woNum = this.eqpinfo1.value["woNum"];
    eqp.h.poId = this.poId;// this.eqpinfo1.value["poId"];//this.eqpinfo1.get('poId').value;
    eqp.h.matnr*/


  onRefreshPo() {
    var po = {
      poId: null,
      no: null,
      matnr: null,
      matnrId: null,
      poQty: null,
      description: null,
      deliveryDate: null,
      poNo: null,
      ekgry: null,
      ekgryid: null,
    }

    //alert('1  this.eqpinfo1.value["no"] = ' + this.eqpinfo1.value["no"])
    //alert('2  this.eqpinfo1.get("no")?.value =' + this.eqpinfo1.get("no")?.value)
    //alert('3  this.no  =' + this.no)
    //var eqp1 = this.geteqp();

    var eqp1 = this.geteqp();
    console.log('-------------onRefreshPo-------------')
    console.log(eqp1)

    var qwoNum = eqp1.h.woNum;//this.eqpinfo1.get('woNum').value;//this.eqpinfo1.get("woNum")?.value;
    var qmatnr = eqp1.h.matnr;//this.eqpinfo1.get('matnr').value;//this.eqpinfo1.get("matnr")?.value;
    var qno = eqp1.h.no;//this.eqpinfo1.get('no').value;//this.eqpinfo1.value["no"];//this.eqpinfo1.get("no")?.value;
    var eqp = {
      woNum: qwoNum,
      matnr: qmatnr,
      no: qno,
      vendor: this._storageService.userName
    }
    this.poId = '';
    this.no = '';
    this.matnr = '';
    this.matnrId = '';
    this.poQty = '';
    this.description = '';
    this.deliveryDate = '';
    this.poNo = '';
    this.ekgry = '';
    this.ekgryid = '';

    this.eqpinfo1.patchValue({
      poId: this.poId,
      matnr: this.matnr,
      no: this.no,
      matnrId: this.matnrId,
      poQty: this.poQty,
      description: this.description,
      deliveryDate: this.deliveryDate,
      poNo: this.poNo,
      ekgry: this.ekgry,
      ekgryid: this.ekgryid,

    });

    //this.eqpinfo1.value["matnr"] = "";
    //this.eqpinfo1.value["no"] = "";

    //this.eqpinfo1.get('matnr').value("")
    //this.eqpinfo1.get('no').value("")




    //alert('qwoNum = ' + qwoNum);
    //alert('qmatnr = ' + qmatnr);
    //alert('qno = ' + qno);

    if ((qwoNum != '') && (qwoNum != null)) {
      if (((qmatnr != null) && (qmatnr != '')) || ((qno != null) && (qno != ''))) {
        console.log('----search-----');

        this.eqpinfo1.value["matnr"] = "";
        this.eqpinfo1.value["no"] = "";

        this._srmEqpService.GetPoData(eqp).subscribe(result => {
          console.log('----------GetPoData-------------');
          console.log(result[0]["matnr"]);
          //if (result[0]["po_id"] != null) {
          //alert('bb')
          /*po.poId = result[0]["po_id"];

          po.no = result[0]["polid"];
          po.matnr = result[0]["matnr"];
          po.matnrId = result[0]["matnr_id"];
          po.poQty = result[0]["qty"];
          po.description = result[0]["description"];
          po.deliveryDate = result[0]["deliverydate"];
          po.poNo = result[0]["po_num"];
          po.ekgry = result[0]["ekgry_desc"];
          po.ekgryid = result[0]["ekgry_id"];*/


          this.poId = result[0]["po_id"];
          this.no = result[0]["polid"];
          this.matnr = result[0]["matnr"];
          this.matnrId = result[0]["matnr_id"];
          this.poQty = result[0]["qty"];
          this.description = result[0]["description"];
          this.deliveryDate = result[0]["deliverydate"];
          this.poNo = result[0]["po_num"];
          this.ekgry = result[0]["ekgry_desc"];
          this.ekgryid = result[0]["ekgry_id"];

          this.eqpinfo1.patchValue({
            poId: this.poId,
            matnr: this.matnr,
            no: this.no,
            matnrId: this.matnrId,
            poQty: this.poQty,
            description: this.description,
            deliveryDate: this.deliveryDate,
            poNo: this.poNo,
            ekgry: this.ekgry,
            ekgryid: this.ekgryid,
      
          });


          /*if ((this.poId == null) || (this.poId == "")) {
            alert('po not fount')
            this.eqpinfo1.value["matnr"] = "";
            this.eqpinfo1.value["no"] = "";
            this.matnr = "";
            this.no = "";
            alert('查無對應採購單資訊!')
          }*/
          //}
        });
      }
      else {
        alert("料號及序號請擇一輸入!")
      }
    }
    else {
      alert("請輸入採購單號!");
    }
    this.eqpinfo1.value["poId"] = this.poId;
    this.eqpinfo1.value["no"] = this.no;
    this.eqpinfo1.value["matnr"] = this.matnr;
    this.eqpinfo1.value["matnrId"] = this.matnrId;
    this.eqpinfo1.value["poQty"] = this.poQty;
    this.eqpinfo1.value["description"] = this.description;
    this.eqpinfo1.value["deliveryDate"] = this.deliveryDate;
    this.eqpinfo1.value["poNo"] = this.poNo;
    this.eqpinfo1.value["ekgry"] = this.ekgry;
    this.eqpinfo1.value["ekgryid"] = this.ekgryid;
  }

  openFile() {
    this.werks = 3100;
    console.log('---------------openFile--------------')
    console.log()
    if (this.eqpNum == undefined) {
      const data = {
        functionId: 9,
        number: this.guid == undefined ? Guid.create().toString() : this.guid,
        werks: this.werks,
        type: 2,
        isUpload: true,
      }
      this.guid = data.number;

      this.filemodal.upload(data);
    }
    else {
      const data = {
        functionId: 9,
        number: this.eqpNum.toString(),
        werks: this.werks,
        type: 2,
        isUpload: true,
      }
      this.guid = data.number;
      console.log('13245');
      console.info(data);
      console.info(this.filemodal);
      this.filemodal.upload(data);
    }
  }
}
