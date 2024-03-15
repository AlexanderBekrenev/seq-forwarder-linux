#!/bin/bash
VERSION="2.1.128.2"
ARCH="amd64"

if [[ $1 =~ ^(http:\/\/|https:\/\/)[a-z0-9\-]+(\.[a-z0-9\-]+)*(:[0-9]{1,5})?(\/.*)?$ ]]
then
    URL=$1
fi

if [ -z "$VERSION" ]; then
    echo "No build version"
    exit 1
fi
if [[ -n $URL ]]
then
    VERSION="custom-$VERSION"
fi

echo "Build DEB-package for seqfwd version $VERSION"

rm -rf ./publish.seqfwd

dotnet publish ../src/Seq.Forwarder/Seq.Forwarder.csproj --self-contained --framework net8.0 --configuration Publish -o ./publish.seqfwd -p:Version="$VERSION"
rc=$?
if [[ ${rc} -ne 0 ]]
then
  echo "Error ($rc)"
  exit $rc
else
  echo "Publish directory is ready"
fi

rm -rf seqfwd-$VERSION-$ARCH
mkdir -p seqfwd-$VERSION-$ARCH/usr/lib/seqfwd
cp -r ./publish.seqfwd/* seqfwd-$VERSION-$ARCH/usr/lib/seqfwd
cp -r seqfwd.template/* seqfwd-$VERSION-$ARCH
#update version in control file
sed -i "s/Version:X\.X/Version:$VERSION/g" seqfwd-$VERSION-$ARCH/DEBIAN/control
if [[ -n $URL ]]
then
  echo "Set server url to $URL"
  sed -i "s|\-\-url=http:\/\/localhost:5341|--url=$URL|gm" seqfwd-$VERSION-$ARCH//etc/systemd/system/seqfwd.service.d/override.conf
fi 
chmod 644 "seqfwd-$VERSION-$ARCH/lib/systemd/system/seqfwd.service"


if ! dpkg-deb --root-owner-group -v --build seqfwd-$VERSION-$ARCH
then
    exit 3
fi
echo "Remove build directory"
rm -rf seqfwd-$VERSION-$ARCH

echo "DEB-package for seqfwd version $VERSION: seqfwd-$VERSION-$ARCH.deb"
exit 0

