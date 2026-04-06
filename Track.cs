namespace scrobbler;
using System;

public record Track
(
string Artist,
string Album,
string Title,
int TrackNum,
int Length,
char Rating,
int TimeStamp,
string TrackId,
bool scrobbled
);