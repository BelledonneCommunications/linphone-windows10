############################################################################
# CMakeLists.txt
# Copyright (C) 2015  Belledonne Communications, Grenoble France
#
############################################################################
#
# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU General Public License
# as published by the Free Software Foundation; either version 2
# of the License, or (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
#
############################################################################

cmake_minimum_required(VERSION 2.8)
project(FFMPEG C)

set(LIBAVCODEC_HEADER_FILES
	include/libavcodec/avcodec.h
	include/libavcodec/avfft.h
	include/libavcodec/d3d11va.h
	include/libavcodec/dv_profile.h
	include/libavcodec/dxva2.h
	include/libavcodec/old_codec_ids.h
	include/libavcodec/qsv.h
	include/libavcodec/vaapi.h
	include/libavcodec/vda.h
	include/libavcodec/vdpau.h
	include/libavcodec/version.h
	include/libavcodec/videotoolbox.h
	include/libavcodec/vorbis_parser.h
	include/libavcodec/xvmc.h
)

set(LIBAVUTIL_HEADER_FILES
	include/libavutil/adler32.h
	include/libavutil/aes.h
	include/libavutil/attributes.h
	include/libavutil/audio_fifo.h
	include/libavutil/audioconvert.h
	include/libavutil/avassert.h
	include/libavutil/avconfig.h
	include/libavutil/avstring.h
	include/libavutil/avutil.h
	include/libavutil/base64.h
	include/libavutil/blowfish.h
	include/libavutil/bprint.h
	include/libavutil/bswap.h
	include/libavutil/buffer.h
	include/libavutil/camellia.h
	include/libavutil/cast5.h
	include/libavutil/channel_layout.h
	include/libavutil/common.h
	include/libavutil/cpu.h
	include/libavutil/crc.h
	include/libavutil/dict.h
	include/libavutil/display.h
	include/libavutil/downmix_info.h
	include/libavutil/error.h
	include/libavutil/eval.h
	include/libavutil/ffversion.h
	include/libavutil/fifo.h
	include/libavutil/file.h
	include/libavutil/frame.h
	include/libavutil/hash.h
	include/libavutil/hmac.h
	include/libavutil/imgutils.h
	include/libavutil/intfloat.h
	include/libavutil/intreadwrite.h
	include/libavutil/lfg.h
	include/libavutil/log.h
	include/libavutil/macros.h
	include/libavutil/mathematics.h
	include/libavutil/md5.h
	include/libavutil/mem.h
	include/libavutil/motion_vector.h
	include/libavutil/murmur3.h
	include/libavutil/old_pix_fmts.h
	include/libavutil/opt.h
	include/libavutil/parseutils.h
	include/libavutil/pixdesc.h
	include/libavutil/pixelutils.h
	include/libavutil/pixfmt.h
	include/libavutil/random_seed.h
	include/libavutil/rational.h
	include/libavutil/replaygain.h
	include/libavutil/ripemd.h
	include/libavutil/samplefmt.h
	include/libavutil/sha.h
	include/libavutil/sha512.h
	include/libavutil/stereo3d.h
	include/libavutil/tea.h
	include/libavutil/threadmessage.h
	include/libavutil/time.h
	include/libavutil/timecode.h
	include/libavutil/timestamp.h
	include/libavutil/twofish.h
	include/libavutil/version.h
	include/libavutil/xtea.h
)

set(LIBSWRESAMPLE_HEADER_FILES
	include/libswresample/swresample.h
	include/libswresample/version.h
)

set(LIBSWSCALE_HEADER_FILES
	include/libswscale/swscale.h
	include/libswscale/version.h
)

install(FILES ${LIBAVCODEC_HEADER_FILES}
	DESTINATION include/libavcodec
	PERMISSIONS OWNER_READ OWNER_WRITE GROUP_READ WORLD_READ
)

install(FILES ${LIBAVUTIL_HEADER_FILES}
	DESTINATION include/libavutil
	PERMISSIONS OWNER_READ OWNER_WRITE GROUP_READ WORLD_READ
)

install(FILES ${LIBSWRESAMPLE_HEADER_FILES}
	DESTINATION include/libswresample
	PERMISSIONS OWNER_READ OWNER_WRITE GROUP_READ WORLD_READ
)

install(FILES ${LIBSWSCALE_HEADER_FILES}
	DESTINATION include/libswscale
	PERMISSIONS OWNER_READ OWNER_WRITE GROUP_READ WORLD_READ
)

set(arch "x86")
if(CMAKE_GENERATOR MATCHES "^.* ARM$")
	set(arch "ARM")
elseif(CMAKE_GENERATOR MATCHES "^.* Win64$")
	set(arch "x64")
endif()

install(FILES ${arch}/avcodec.lib ${arch}/avutil.lib ${arch}/swresample.lib ${arch}/swscale.lib
	DESTINATION lib
	PERMISSIONS OWNER_READ OWNER_WRITE GROUP_READ WORLD_READ
)

install(FILES ${arch}/avcodec-56.dll ${arch}/avutil-54.dll ${arch}/swresample-1.dll ${arch}/swscale-3.dll
	DESTINATION bin
	PERMISSIONS OWNER_READ OWNER_WRITE GROUP_READ WORLD_READ
)
