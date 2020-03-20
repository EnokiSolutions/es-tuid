# ES.Tuid

## What is this?

A TUID is like a UUID (it conforms to UUID v4) but instead of being fully random (except for 6 bits for the version)
it is prefixed with the time since epoch in microseconds, a sequence number (if generating more then one per
microsecond, or waiting for clock rollback to catch up), and node id.

## Why use a TUID instead of a UUIDv4?

The high bits in a UUIDv4 are random, so if you have a data with a lot of entries using UUIDv4 in an index it leads
to some performance issues. The main issue is new data being inserted can cause an update to an index in a random
location. This defeats caching of index entries and leads to performance issues. By ensuring the ids are generally
monotonically increasing the entries added will be locally at the "head" of the index and multiple inserts will 
benefit from cache locality. (This benefit also extends in general, in that most data that is related is created at
the same time.)

## Discussion

The use of a node id and sequence number was included more to deal with political "it could collide, so it's unsafe"
arguments that have been used against UUIDs in the past. If you _absolutely_ require the IDs be unique then use the pg_tuid 
`tuid_generate` postgresql extension (http://github.com/tanglebones/pg_tuid) and assign each DB instance a unique node id 
and only generate them using the DB. This is still better than using an auto-incrementing integer because the TUIDs are not
in a compact space (as there are 42+ bits of randomness added), and therefore cannot be easily guessed by an attacker.

The C# client code example correctly handles thread safety and clock roll back (using sequence numbering if the clock time
goes backwards to allow for faster catch up). It also picks the starting sequence number at random so in most cases the
level of randomness is increased.

Even though the C# client code handles clock roll back you should assume there different machines could generate TUIDs
out of sync since each is using their own clock there will be drift in the prefixes; this will be true **even** if you use a
time synchronization system (NTPD, etc.) as a true synchronization is impossible. Time drift is usually not a problem unless
you're relying on the order of the ids to be absolutely ascending. If you really need the IDs to be absolutely ascending use
the DB based solution.
