import os
import base64
from hashlib import pbkdf2_hmac

pwds = ['admin123', 'user123', 'user1234']
for pwd in pwds:
    salt = os.urandom(16)
    h = pbkdf2_hmac('sha256', pwd.encode(), salt, 100000, dklen=32)
    print(pwd + ' 100000.' + base64.b64encode(salt).decode() + '.' + base64.b64encode(h).decode())
