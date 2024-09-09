unit oldlinux;

interface

uses
{$ifdef WIN32}
{$endif}
{$ifdef LINUX}
{ ,oldlinux}
{$define UNIX_STYLE}
 baseunix,
 unixtype,
 unix
{$endif}
;

type timeval=record
    SEC:  cLong;
    USEC: cLong;
end;

procedure gettimeofday(var t:timeval);

implementation

{$ifdef UNIX_STYLE}
    procedure gettimeofday(var t:timeval);
    var tz:timezone;
	tv:unixtype.timeval;
    begin
	fpgettimeofday(@tv, @tz);
	t.SEC:=tv.tv_sec;
	t.USEC:=tv.tv_usec;
    end;
{$endif}

end.
