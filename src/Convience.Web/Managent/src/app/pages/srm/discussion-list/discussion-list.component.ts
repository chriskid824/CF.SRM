import { Component, OnInit } from '@angular/core';
import { SrmDisscussionService } from '../../../business/srm/srm-disscussion.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { stringify } from 'node:querystring';
interface SapResult {
  id: string;
  lId: number;
  type: string;
  outCome: string;
  reason: string;
}
@Component({
  selector: 'app-discussion-list',
  templateUrl: './discussion-list.component.html',
  styleUrls: ['./discussion-list.component.less']
})
export class DiscussionListComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  date = null;
  page: number = 1;
  size: number = 6;
  total: number;
  listOfColumn = [
    // {
    //   title: '主題編號',
    //   // compare: (a: SapResult, b: SapResult) => a.id.localeCompare(b.id),
    //   priority: 1
    // },
    {
      title: '類別',
      // compare: (a: SapResult, b: SapResult) => a.type.localeCompare(b.type),
      priority: 3
    },
    {
      title: '單號',
      // compare: (a: SapResult, b: SapResult) => a.outCome.localeCompare(b.outCome),
      priority: 4
    },

    {
      title: '標題',
      // compare: (a: SapResult, b: SapResult) => a.lId - b.lId,
      priority: 2
    },
     {
       title: '作者',
       // compare: (a: SapResult, b: SapResult) => a.reason.localeCompare(b.reason),
       priority: 5
     },
     {
      title: '操作時間',
      // compare: (a: SapResult, b: SapResult) => a.reason.localeCompare(b.reason),
      priority: 6
    }
    // {
    //   title: '操作',
    //   // compare: (a: SapResult, b: SapResult) => a.reason.localeCompare(b.reason),
    //   priority: 5
    // }
  ];
  listOfData: any;
  constructor(private _formBuilder: FormBuilder,private _srmDisscussionService: SrmDisscussionService,private message: NzMessageService) { }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      id: [null],
    });
    this.refresh();
  }
  onChange(result: Date[]): void {
    console.log('onChange: ', result);
  }
  submitSearch()
  {
    this.refresh();
  }
  pageChange() {
    this.refresh();
  }
  refresh()
  {
    var query = {
      id: this.searchForm.value["id"] == null ? "" : this.searchForm.value["id"],
      page: this.page,
      size: this.size
    }
    this._srmDisscussionService.GetDissList(query)
    .subscribe((result) => {
      console.info(result);
      this.listOfData=result["data"];
      this.total=result["count"];
      // if(result["err"]==null)
      // {
      //   this.listOfData=result["list"];
      // }
      // else
      // {

      //   alert(result["err"]);
      // }
    });
  }
  NewTitle(){
    window.location.href='/srm/diss-add';
  }
}
