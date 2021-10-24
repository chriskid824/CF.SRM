import { Component, OnInit,TemplateRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { FileType,Function,BaseSelect,ViewSrmFileUploadTemplate } from '../model/File';
import { SrmFileService } from 'src/app/business/srm/srm-file.service';

@Component({
  selector: 'app-file-template',
  templateUrl: './file-template.component.html',
  styleUrls: ['./file-template.component.less']
})
export class FileTemplateComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  editForm: FormGroup = new FormGroup({});
  passwordSetForm: FormGroup = new FormGroup({});
  isNewUser: boolean;

  editedTemplate: ViewSrmFileUploadTemplate;

  tplModal: NzModalRef;
  listOfColumn = [
    {
      title: '樣板識別碼',
      compare: (a: ViewSrmFileUploadTemplate, b: ViewSrmFileUploadTemplate) => a.templateId-b.templateId,
      priority: 1
    },
    {
      title: '功能識別碼',
      compare: (a: ViewSrmFileUploadTemplate, b: ViewSrmFileUploadTemplate) => a.templateType - b.templateType,
      priority: 2
    },
    {
      title: '廠別',
      compare: (a: ViewSrmFileUploadTemplate, b: ViewSrmFileUploadTemplate) => a.werks-b.werks,
      priority: 3
    },
    {
      title: '廠內/廠外',
      compare: (a: ViewSrmFileUploadTemplate, b: ViewSrmFileUploadTemplate) => a.type-b.type,
      priority: 4
    },
    {
      title: '生效日期',
      compare: (a: ViewSrmFileUploadTemplate, b: ViewSrmFileUploadTemplate) => a.effectiveDate.toLocaleDateString().localeCompare(b.effectiveDate.toLocaleDateString()),
      priority: 5
    },
    {
      title: '截止日期',
      compare: (a: ViewSrmFileUploadTemplate, b: ViewSrmFileUploadTemplate) => a.deadline.toLocaleDateString().localeCompare(b.deadline.toLocaleDateString()),
      priority: 6
    },
    {
      title: '檔案類型',
      priority: 7
    },
    {
      title: '操作',
      priority: 8
    }
  ];
  listOfData: any;
  page: number = 1;
  size: number = 2;
  total: number;



  filetypes: FileType[] = [];
  types: Function[] = [];
  fileTypes: BaseSelect[] = [{id:1,name:"SIP"},{id:2,name:"SOP"},{id:3,name:"ZDR"},{id:4,name:"材證"},{id:5,name:"第三方檢驗文件"},{id:6,name:"進口報關文件"}];
  werkOptions: BaseSelect[] = [{id:1100,name:"廠工事業處"},{id:1200,name:"機械事業處"},{id:3100,name:"精密事業處"}];
  typeOptions: BaseSelect[] = [{id:1,name:"供應商"},{id:2,name:"料號"},{id:3,name:"詢價單"},{id:4,name:"報價單"},{id:5,name:"價格資訊"},{id:6,name:"資訊紀錄"},{id:7,name:"採購單"},{id:8,name:"出貨單"}];
  constructor(
    private _formBuilder: FormBuilder,
    private _messageService: NzMessageService,
    private _modalService: NzModalService,
    private _srmFileService: SrmFileService,) {
}

  ngOnInit(): void {
  }

  submitSearch(){}

  add(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.isNewUser = true;
    //this.editedUser = new SapResult();
    this.editForm = this._formBuilder.group({
      templateId: [''],
      templateType: ['',[Validators.required]],
      werks: ['', [Validators.required]],
      type: ['', [Validators.required]],
      effectiveDate: [''],
      deadline: [''],
      filetypes: [[]],
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
      nzClosable: true,
      nzMaskClosable: false
    });
  }

  edit(title: TemplateRef<{}>, content: TemplateRef<{}>, template: ViewSrmFileUploadTemplate) {
    // this._userService.getUser(user.id).subscribe(user => {
    //   console.log(user['userName']);
       this.isNewUser = false;
       this.editedTemplate = template;
       this.editForm = this._formBuilder.group({
        templateId: [template['templateId']],
        templateType: [template['templateType'],[Validators.required, Validators.maxLength(15)]],
        werks: [template['werks'], [Validators.required, Validators.maxLength(15)]],
        type: [template['type'], [Validators.required, Validators.maxLength(10)]],
        effectiveDate: [template['effectiveDate']],
        deadline: [template['deadline']],
        filetypes: [template['filetypes'].split(',')],
       });
      this.tplModal = this._modalService.create({
        nzTitle: title,
        nzContent: content,
        nzFooter: null,
        nzClosable: true,
        nzMaskClosable: false
      });
    //});
  }

  submitEdit() {
    for (const i in this.editForm.controls) {
      this.editForm.controls[i].markAsDirty();
      this.editForm.controls[i].updateValueAndValidity();
    }

    if (this.editForm.valid) {
      let template: any = {};
      alert(this.editForm.value['templateId']);
      template.templateId = this.editForm.value['templateId'];
      alert(this.editForm.value['templateId']);
      template.templateType=this.editForm.value['templateType'];
      template.werks = this.editForm.value['werks'];
      template.type = this.editForm.value['type'];
      template.effectiveDate = this.editForm.value['effectiveDate'];
      template.deadline = this.editForm.value['deadline'];
      template.filetypes = this.editForm.value['filetypes'];
      if (this.editedTemplate.templateId) {
        alert(111);
        template.templateId = this.editedTemplate.templateId;
        this._srmFileService.UpdateTemplate(template).subscribe(result => {
          this._messageService.success("更新成功！");
          this.tplModal.close();
          this.refresh();
        });
      } else {
alert(222);
        this._srmFileService.AddTemplate(template).subscribe(result => {
          this._messageService.success("添加成功！");
          this.tplModal.close();
          this.refresh();
        });
      }
    }
  }

  refresh() {
    var query = {
      templateId: this.searchForm.value["I_EBELN"] == null ? "" : this.searchForm.value["I_EBELN"],
      werks: this.searchForm.value["I_EKORG"] == null ? "0" : this.searchForm.value["I_EKORG"],
      page: this.page,
      size: this.size
    }
    this._srmFileService.GetTemplateList(query)
      .subscribe(result => {
        this.listOfData = result['data'];
        this.total = result['count'];
      });
  }
}
