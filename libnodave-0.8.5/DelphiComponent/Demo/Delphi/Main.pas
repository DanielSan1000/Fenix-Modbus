// Main.pas (Part of NoDaveDemo.dpr)
//
// A program demonstrating the functionality of the TNoDave component.
// This unit implements the mainform of the application.
//
// (C) 2005 Gebr. Schmid GmbH + Co., Freudenstadt, Germany
//
// Author: Axel Kinting (akinting@schmid-online.de)
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

unit Main;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, IniFiles,
  StdCtrls, Mask, Grids, ValEdit, ExtCtrls, ComCtrls, Winsock, Spin, NoDaveComponent, Editor,
  AppEvnts;

type
  TMainForm = class(TForm)
    StatusBar1: TStatusBar;
    Panel1: TPanel;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Label5: TLabel;
    StaticText1: TStaticText;
    Button1: TButton;
    Button2: TButton;
    ComboBox1: TComboBox;
    Button3: TButton;
    ComboBox2: TComboBox;
    ProgressBar1: TProgressBar;
    SpinEdit1: TSpinEdit;
    SpinEdit2: TSpinEdit;
    SpinEdit3: TSpinEdit;
    NoDave: TNoDave;
    ApplicationEvents1: TApplicationEvents;
    Panel2: TPanel;
    Panel3: TPanel;
    Label6: TLabel;
    ComboBox3: TComboBox;
    Button4: TButton;
    Button5: TButton;
    Panel4: TPanel;
    ListBox1: TListBox;
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure NoDaveRead(Sender: TObject);
    procedure NoDaveError(Sender: TObject; ErrorMsg: string);
    procedure NoDaveConnect(Sender: TObject);
    procedure NoDaveDisconnect(Sender: TObject);
    procedure ListBox1DblClick(Sender: TObject);
    procedure Button4Click(Sender: TObject);
    procedure Button5Click(Sender: TObject);
    procedure FormShow(Sender: TObject);
    procedure ApplicationEvents1Idle(Sender: TObject; var Done: Boolean);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure ComboBox3Select(Sender: TObject);
  private
    DataType: Integer;
    DataOffset: Integer;
    DataCount: Integer;
    DataSize: Integer;
    MustRefresh: Boolean;
  public
    procedure Connect(ConnName: String);
  end;

var
  MainForm: TMainForm;

implementation

{$R *.dfm}

uses SZLView;

const
  AreaNames: Array[0..13] of String = ('SI%1:s%4u', 'SF%1:s%4u', 'I%1:s%4u', 'Q%1:s%4u', 'I%1:s%4u', 'Q%1:s%4u', 'M%1:s%4u', 'DB%u.%s%4u', 'DI%u.%s%4u', 'DL%u.%s%4u', '?%u.%s%4u', 'C%1:s%4u', 'T%1:s%4u', 'PE%1:s%4u');
  TypeNames: Array[0..5] of String = ('B', 'W', 'Int', 'DW', 'DInt', 'Real');

procedure TMainForm.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  NoDave.Active:=False;
end;

procedure TMainForm.FormShow(Sender: TObject);
var
  ConnList: TStringList;
  Index: Integer;
  ConnName: String;
  ConnDesc: String;
begin
  ConnList:=TStringList.Create;
  ConnectionEditor.IniFile.ReadSectionValues('Connections', ConnList);
  ConnList.Sorted:=True;
  Index:=0;
  While Index < ConnList.Count do
  begin
    ConnName:=ConnList.Names[Index];
    ComboBox3.Items.Append(ConnName);
    Inc(Index);
  end;
  ConnList.Free;
  ComboBox3.ItemIndex:=0;
  Connect(ComboBox3.Text);
end;

procedure TMainForm.Button1Click(Sender: TObject);
var
  Active: Boolean;
  Area: Integer;
  DBNumber: Integer;
  Index: Integer;
  Display: String;
begin
  Active:=NoDave.Active;
  Area:=ComboBox2.ItemIndex;
  NoDave.Area:=TNoDaveArea(Area);
  DBNumber:=SpinEdit1.Value;
  NoDave.DBNumber:=DBNumber;
  DataType:=Combobox1.ItemIndex;
  DataCount:=SpinEdit2.Value;
  DataOffset:=SpinEdit3.Value;
  Case DataType of
    0: DataSize:=1;
    1: DataSize:=2;
    2: DataSize:=2;
    3: DataSize:=4;
    4: DataSize:=4;
    5: DataSize:=4;
  end;
  NoDave.BufLen:=DataCount * DataSize;
  NoDave.BufOffs:=DataOffset;
  ListBox1.Clear;
  Index:=DataOffset;
  While Index < DataOffset + DataCount do
  begin
    Display:=StringReplace(Format(AreaNames[Area], [DBNumber, TypeNames[DataType], Index]), ' ', '0', [rfReplaceAll]) + ' =         ?';
    ListBox1.Items.Append(Display);
    Inc(Index);
  end;
  NoDave.Active:=Active;
end;

procedure TMainForm.Button2Click(Sender: TObject);
var
  WasActive: Boolean;
begin
  WasActive:=NoDave.Active;
  NoDave.Active:=True;
  SZLViewer.Show;
  NoDave.Active:=WasActive;
end;

procedure TMainForm.ListBox1DblClick(Sender: TObject);
var
  Index: Integer;
  VName: String;
  Value: String;
  Address: Integer;
begin
  Index:=ListBox1.ItemIndex;
  VName:=ListBox1.Items.Names[Index];
  Value:=ListBox1.Items.Values[VName];
  While (Length(Value) > 0) and (Value[1] = ' ') do Delete(Value, 1, 1);
  Value:=InputBox(VName, 'New Value:', Value);
  Address:=((Index) * DataSize) + DataOffset;
  try
    Case DataType of
      0: NoDave.WriteByte(Address, StrToInt(Value));
      1: NoDave.WriteWord(Address, StrToInt(Value));
      2: NoDave.WriteInt(Address, StrToInt(Value));
      3: NoDave.WriteDWord(Address, StrToInt(Value));
      4: NoDave.WriteDInt(Address, StrToInt(Value));
      5: NoDave.WriteFloat(Address, StrToFloat(Value));
    end;
  except
  end;
end;

procedure TMainForm.Button3Click(Sender: TObject);
begin
  NoDave.Active:=not NoDave.Active;
end;

procedure TMainForm.NoDaveError(Sender: TObject; ErrorMsg: string);
begin
  StaticText1.Caption:='Last cycle time: - ms';
  StatusBar1.SimpleText:='Error [' + IntToStr(NoDave.LastError) + ']: ' + ErrorMsg;
end;

procedure TMainForm.NoDaveRead(Sender: TObject);
var
  Data: Array of Variant;
  Index: Integer;
  Count: Integer;
  Item: String;
  Display: String;
begin
  With ProgressBar1 do
  begin
    If Position = 10 then Position:=0 else Position:=Position + 1;
  end;
  SetLength(Data, DataCount);
  Count:=0;
  While Count < DataCount do
  begin
    Index:=(Count * DataSize) + DataOffset;
    Case DataType of
      0: Data[Count]:=NoDave.GetByte(Index);
      1: Data[Count]:=NoDave.GetWord(Index);
      2: Data[Count]:=NoDave.GetInt(Index);
      3: Data[Count]:=NoDave.GetDWord(Index);
      4: Data[Count]:=NoDave.GetDInt(Index);
      5: Data[Count]:=NoDave.GetFloat(Index);
    end;
    Inc(Count);
  end;
  Index:=0;
  While Index < DataCount do
  begin
    Item:=ListBox1.Items.Names[Index];
    Display:=' ' + Format('%10s', [VarToStr(Data[Index])]);
    If Display <> ListBox1.Items.Values[Item] then ListBox1.Items.Values[Item]:=Display;
    Inc(Index);
  end;
  StaticText1.Caption:='Last cycle time: ' + IntToStr(NoDave.CycleTime) + ' ms';
  StatusBar1.SimpleText:='';
end;

procedure TMainForm.NoDaveConnect(Sender: TObject);
begin
  ProgressBar1.Position:=0;
  ListBox1.Enabled:=True;
end;

procedure TMainForm.NoDaveDisconnect(Sender: TObject);
begin
  ProgressBar1.Position:=0;
  ListBox1.Enabled:=False;
end;

procedure TMainForm.Connect(ConnName: String);
begin
  If ConnName <> '' then
  begin
    NoDave.Protocol:=TNoDaveProtocol(ConnectionEditor.IniFile.ReadInteger(ConnName, 'Protocol', 3));
    NoDave.CPURack:=ConnectionEditor.IniFile.ReadInteger(ConnName, 'CPURack', 0);
    NoDave.CPUSlot:=ConnectionEditor.IniFile.ReadInteger(ConnName, 'CPUSlot', 2);
    NoDave.COMPort:=ConnectionEditor.IniFile.ReadString(ConnName, 'COMPort', 'COM1:');
    NoDave.IPAddress:=ConnectionEditor.IniFile.ReadString(ConnName, 'IPAddress', '');
    NoDave.IntfTimeout:=ConnectionEditor.IniFile.ReadInteger(ConnName, 'Timeout', 100000);
    NoDave.Interval:=ConnectionEditor.IniFile.ReadInteger(ConnName, 'Interval', 1000);
    NoDave.MPISpeed:=TNoDaveSpeed(ConnectionEditor.IniFile.ReadInteger(ConnName, 'MPISpeed', 2));
    NoDave.MPILocal:=ConnectionEditor.IniFile.ReadInteger(ConnName, 'MPILocal', 1);
    NoDave.MPIRemote:=ConnectionEditor.IniFile.ReadInteger(ConnName, 'MPIRemote', 2);
//    Button1Click(Self);
  end;
end;

procedure TMainForm.Button4Click(Sender: TObject);
begin
  If ComboBox3.ItemIndex >= 0 then
  begin
    ConnectionEditor.SetConnection(ComboBox3.Text);
    If ConnectionEditor.ShowModal = mrOK then Connect(ComboBox3.Text);
  end;
end;

procedure TMainForm.Button5Click(Sender: TObject);
var
  Index: Integer;
begin
  ConnectionEditor.SetConnection('');
  If ConnectionEditor.ShowModal = mrOK then
  begin
    Index:=ComboBox3.Items.Add(ConnectionEditor.Connection.Text);
    ComboBox3.ItemIndex:=Index;
    Connect(ComboBox3.Text);
  end;
end;

procedure TMainForm.ApplicationEvents1Idle(Sender: TObject; var Done: Boolean);
begin
  If NoDave.Active then Button3.Caption:='Stop' else Button3.Caption:='Start';
end;

procedure TMainForm.ComboBox3Select(Sender: TObject);
begin
  Connect(ComboBox3.Text);
end;

end.


// 03.05.2005 17:12:52 [C:\Programme\Borland\Delphi6\User\GsfExperts\NoDave\Demo\Delphi\Main.pas]  Projekt Version: 1.0.0.1
//     Created !
// 28.11.2005 10:50:42 [C:\Programme\Borland\Delphi6\User\GsfExperts\NoDave\Demo\Delphi\Main.pas]  Projekt Version: 1.0.0.1
//     SZL-Viewer added.
