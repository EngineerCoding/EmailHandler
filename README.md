# EmailHandler

This repository contains the software stack to handle incoming mail and manage settings for that. The infrastructure is based on a [postfix](http://www.postfix.org/) installation which pipes incoming mail to the `MailHandler` project which sends to email to an offsite handler. This is applied since I don't want to be concerned with storage issues, which I therefore offload to a commercial provider which handles it for me. They have data redundancy setup so that I don't have to buy hardware to support this.


While currently this is not fully implemented yet, the flow will eventually look like this:
```
           +----------------+
           | Incoming email |
           +----------------+
                   |
                   v
  +----------------------------------+  no   +-------------------------------+  yes   +-----+
  | Is from offloaded email address? | ----> | is X-Original-To blacklisted? | -----> | End |
  +----------------------------------+       +-------------------------------+        +-----+
                  | yes                                     |
                  v                                         v
     +---------------------------+           +-------------------------------+
     | Parse 1st metadata block  |           | Generate metadata about email |
 +---| Remove 1st metadata block |           +-------------------------------+
 |   +---------------------------+                          |
 |                |                                         v
 |                v                          +-------------------------------+
 |         +--------------+                  |   Insert metadata in email    |
 |         | Is replying? |                  |        (html and text)        |
 |         +--------------+                  +-------------------------------+
 |                | no                                      |
 | yes            v                                         v
 |     +------------------------+                 +---------------------+
 |     | Set correct message ID |                 | Add tags to subject |
 |     +------------------------+                 +---------------------+
 |                |                                         |
 |                v                                         v
 |  +------------------------------+        +---------------------------------+
 +->| Send email as specified user |        | Send to offloaded email address |
    +------------------------------+        +---------------------------------+
```

Whether the `postfix` header is blacklisted is determined by a small backing database, which also serves for the storage whether a tag is added to the subject line.

The email handler contains the following arguments:

*... to be refined and documented ...*

To be implemented:

- Sending emails back through the handlers (right hand side of the flow depicted above)

## Postfix installation

This email handler runs a simple postfix installation, for which several guides can be found on the internet. *I also have written about it in my blog (**not yet written**), which could help you getting started.* The incoming emails are piped to the `mailbox_command` settings, which in my setup runs the `MailHandler` program. This is alos the place where arguments to the program are set.

# Backing web API

A small backing API exists to update the database on the server, which is currently is for both blacklisting emails and tagging subject lines. This is a simple CRUD API for entries which contain the following properties:

* **EmailUser**: the user part of incoming email-address (everything before the @)
* **Blacklisted**: whether the user is blacklisted and email should not be forwarded
* **Tag**: the tag associated with this user
