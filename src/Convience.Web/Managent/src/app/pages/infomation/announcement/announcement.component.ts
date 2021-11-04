import { HttpClient } from '@angular/common/http';
import { AfterViewInit, Component, ViewChild, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator'
import { TestBed } from '@angular/core/testing';
import { DataSource } from '@angular/cdk/collections';
import { AnnouncementType,Main_Announcement } from './announcement.model';

@Component({
  selector: 'app-announcement',
  templateUrl: './announcement.component.html',
  styleUrls: ['./announcement.component.less']
})
export class AnnouncementComponent implements AfterViewInit {
  @Input() announcementType: AnnouncementType=new AnnouncementType("black","其他","mdi-bullhorn-outline",[],[],null);
  getClass(): string {
    switch (this.announcementType.stylecolor) {
      case "red":
        return "announce_red";
      case "blue":
        return "announce_blue";
      case "green":
        return "announce_green";
      case "lightgreen":
        return "announce_lightgreen";
      case "orange":
        return "announce_orange";
      default:
        return "announce_black";
    }
  }
  DataSource = new MatTableDataSource<any>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  columndefs: any[] = ['StartDateTime', 'txtTypeName', 'txtSubject', 'txtSN', 'txtUser', 'txtExt'];
  constructor(private httpClient: HttpClient) {
    let data = {
      route: 'api/EIP/GetAnnList',
      data: { logonid: sessionStorage.getItem('logonid_o') }
    }
    var succ = (result: any): void => {
      this.DataSource = new MatTableDataSource(result.data.data);
      this.DataSource.paginator = this.paginator;
    }
    var err = (result: any): void => {
      alert(result.message)
    }
    //utility.CallWebAPI(httpClient, data, succ, err);
  }

  ngAfterViewInit(): void {
    //   this.paginator._intl.itemsPerPageLabel = "每頁筆數";
    // this.DataSource.paginator = this.paginator;
  }
  public openDialog(title: string, content: string, type: string, viewStart: string, fileInfo: string, txtSN: string, txtUser: string, txtExt: string) {

    //this.dialog.open(AnnPageComponent, dialogConfig);
  }
public getLink(number):string
{
  return this.announcementType.router+'/'+number;
}
  public open(row: any) {
    let data = {
      route: 'api/EIP/GetAnn',
      data: { caseid: row.CASEID }
    }
    var succ = (result: any): void => {
      console.log(result.data);
      this.openDialog(result.data.title, result.data.content, result.data.type, result.data.startView, result.data.fileInfo, row.txtSN, row.txtUser, row.txtExt);
    }
    var err = (result: any): void => {
      alert(result.message)
    }
    //utility.CallWebAPI(this.httpClient, data, succ, err);



    // let body = { route: 'api/EIP/GetAnn', data: { caseid: caseid} };
    // let url = "http://10.1.1.180/CF.BPM.Service/api/Utility/CallWebAPI_returnModel";
    // this.httpClient.post<any>(url, body).subscribe(res => {
    //   if (res.status) {
    //         this.openDialog(res.data.title,res.data.content);
    //   }
    // });
  }

}
