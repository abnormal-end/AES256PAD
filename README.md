# AES256PAD
AES256PAD is a text editor that uses AES256-GCM encryption to secure your data.

Use it to store sensitive data in the cloud or on USB-Stick

### How secure is it?
AES with 256 bits key lengths is one of the most robust encryption algorithm. Its use is widespread.

GCM has been proven secure and works nice with AES.

But keep in mind that your data will only be secure if:
- the password you used for encryption is strong and private
- the computer you are using is clean (no keyloggers, virus, ...)

### What technology is used?
AES256PAD is written in C# .NET Framework 4. It's build around jbtule/AESGCM.cs implementation: https://gist.github.com/jbtule/4336842 (which itself uses BouncyCastle library).

AESGCM.cs is a StackOverflow reviewed AES256-GCM implementation: http://stackoverflow.com/questions/202011/encrypt-and-decrypt-a-string/10366194#10366194

### Features
There is no other feature than encryption (and a basic search box).

But no feature is a feature itself: It allows you to review the code easily and check that no hidden troll is stealing your data!

### But I need feature X
Fork :)

### How can I build the .exe myself?
Grab the sources, add a reference to BouncyCastle library (available on nuget), target Framework 4, compile.
